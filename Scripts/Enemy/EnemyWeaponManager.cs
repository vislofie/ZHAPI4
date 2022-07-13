using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponManager : WeaponManager
{
    Transform target;
    [SerializeField][Tooltip("How much shoot ray is going to be offset")]
    private float _aimOffset;

    public void Initialize()
    {
        _player = false;
    }

    public void AimAndShoot(Transform targetToAim)
    {
        target = targetToAim;
        if (_initialized)
        {
            if (Physics.Raycast(_revolver.position, ((targetToAim.position + Vector3.one * Random.Range(-_aimOffset, _aimOffset)) - _revolver.position).normalized, out _hitbox, _shootDistance, _shootLayerMask))
            {
                if (_hitbox.collider.TryGetComponent(out PlayerHitbox hitbox))
                {
                    _hitTheHitbox = true;
                }
            }
            else
            {
                _hitTheHitbox = false;
            }
        }
    }
}