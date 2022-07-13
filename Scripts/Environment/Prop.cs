using UnityEngine;

public class Prop : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayRicochetSound()
    {
        if (Random.Range(1, 6) == 1)
            _audioSource.Play();
    }
}
