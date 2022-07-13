using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    [SerializeField]
    private float _radiusOfExplosion = 3.0f;

    [SerializeField]
    private AudioClip _fuseClip;
    [SerializeField]
    private AudioClip _explodingClip;

    private Rigidbody _rigidBody;
    private AudioSource _audioSource;

    private MeshRenderer _meshRenderer;
    private GameObject _fireParticles;
    private GameObject _explosionParticles;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();

        _meshRenderer = GetComponent<MeshRenderer>();

        _fireParticles = transform.GetChild(0).gameObject;
        _explosionParticles = transform.GetChild(1).gameObject;
    }

    public void Activate()
    {
        _meshRenderer.enabled = true;
        _fireParticles.SetActive(true);
        _explosionParticles.SetActive(false);

        _audioSource.clip = _fuseClip;
        _audioSource.Play();
    }

    public void Deactivate()
    {
        _meshRenderer.enabled = false;
        _fireParticles.SetActive(false);
        _explosionParticles.SetActive(true);
    }

    public void Explode(float timeToExplode)
    {
        StopAllCoroutines();
        StartCoroutine(ExplodeBaBoom(timeToExplode));
    }

    private IEnumerator ExplodeBaBoom(float timeToExplode)
    {
        yield return new WaitForSeconds(timeToExplode);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _radiusOfExplosion);

        foreach (Collider collider in colliders)
        {
            if (collider != GetComponent<Collider>())
            {
                if (collider.tag == "Enemy")
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    collider.GetComponent<EnemyBrain>().DetectShot(1500.0f / distance);
                    collider.GetComponent<EnemyBrain>().ShootOffInDirection((collider.transform.position - transform.position).normalized * (650.0f / Mathf.Clamp(distance * distance, 1, distance * distance)));
                }
                else if (collider.tag == "Player")
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    collider.GetComponent<PlayerBrain>().DetectShot(600.0f / Vector3.Distance(transform.position, collider.transform.position));
                    collider.GetComponent<PlayerBrain>().ShootOffInDirection((collider.transform.position - transform.position).normalized * (200.0f / (Mathf.Clamp(distance * distance, 1, distance * distance))));
                }
                else if (collider.tag == "PIProp")
                {
                    collider.GetComponent<Rigidbody>().AddExplosionForce(2000.0f, transform.position, _radiusOfExplosion);
                }
                
            }
                
        }

        _audioSource.Stop();
        _audioSource.clip = _explodingClip;
        _audioSource.Play();
        Deactivate();
    }
}
