using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : WeaponManager
{
    [SerializeField][Tooltip("Create dynamite object in the hierarchy and disable its mesh renderer and particle system. Then attach to this script")]
    private Transform _dynamite;
    [SerializeField]
    private Transform _dynamiteHoldingPlace;



    private MeshRenderer _targetRenderer;
    private Material _normalAimMaterial;
    private Material _enemyAimMaterial;

    public void Initialize(Transform targetTransform, Material normalAimMaterial, Material enemyAimMaterial)
    {
        this.Initialize(targetTransform);

        _targetRenderer = targetTransform.GetComponent<MeshRenderer>();

        _normalAimMaterial = normalAimMaterial;
        _enemyAimMaterial = enemyAimMaterial;

        _player = true;
    }

    public void GetDynamiteInHand()
    {
        _dynamite.GetComponent<Dynamite>().Activate();

        _dynamite.GetComponent<Rigidbody>().isKinematic = true;

        _dynamite.parent = _dynamiteHoldingPlace;

        _dynamite.localPosition = Vector3.zero;
        _dynamite.localRotation = Quaternion.identity;
    }

    public void ThrowDynamite()
    {
        _dynamite.parent = null;

        Rigidbody rb = _dynamite.GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.AddForce((_targetTransform.position - _dynamiteHoldingPlace.position).normalized * 30.0f);
        _dynamite.GetComponent<Dynamite>().Explode(2.5f);
    }

    private void Update()
    {
        if (_initialized)
        {
            if (Physics.Raycast(Camera.main.transform.position, (_targetTransform.position - Camera.main.transform.position).normalized, out _hitbox, _shootDistance, _shootLayerMask))
            {
                if (_hitbox.collider.TryGetComponent(out EnemyHitbox hitbox))
                {
                    _hitTheHitbox = true;
                    _targetRenderer.material = _enemyAimMaterial;
                }
                else
                {
                    _targetRenderer.material = _normalAimMaterial;
                }
            }
            else
            {
                _hitTheHitbox = false;
                _targetRenderer.material = _normalAimMaterial;
            }
        }
    }
}
