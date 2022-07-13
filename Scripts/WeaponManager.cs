using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponManager : MonoBehaviour
{
    [Header("Set up")]
    [SerializeField]
    protected Transform _revolver;
    [SerializeField]
    protected float _shootDistance = 15.0f;
    [SerializeField]
    protected LayerMask _shootLayerMask;
    protected Animator _revolverAnimator;

    protected Transform _targetTransform;

    protected bool _initialized;

    protected bool _hitTheHitbox;
    protected RaycastHit _hitbox;

    [SerializeField]
    private RigController _weaponRig;
    [SerializeField]
    private RigBuilder _rigBuilder;

    [Header("")]
    [Tooltip("Transform of a hand that is holding the gun")]
    [SerializeField]
    private Transform _handTransform;

    [Tooltip("Transform of an object that serves as sheath point")]
    [SerializeField]
    private Transform _sheathTransform;

    [Tooltip("Transform of an object that serves as drawn revolver point")]
    [SerializeField]
    private Transform _drawnTransform;

    private bool _drawn;
    public bool Drawn => _drawn;

    private bool _hammered;

    protected bool _player;
    public bool Player => _player;  

    public void Initialize(Transform targetTransform)
    {
        _initialized = true;

        _targetTransform = targetTransform;

        _revolverAnimator = _revolver.GetComponent<Animator>();
        _hammered = false;
    }

    /// <summary>
    /// Start increasing attached to weapon rig weight
    /// </summary>
    public void Draw()
    {
        Debug.Log("Draw");
        _drawn = true;
        _weaponRig.EnableRig(false, _rigBuilder);
    }

    /// <summary>
    /// Start decreasing attached to weapon rig weight
    /// </summary>
    public void Sheath()
    {
        Debug.Log("Sheath");
        _drawn = false;
        _weaponRig.DisableRig(false, _rigBuilder);
    }

    /// <summary>
    /// Change weapon's parent to let it control the hand
    /// </summary>
    public void ActivateWeapon()
    {
        _revolver.parent = _drawnTransform;
        _revolver.localPosition = Vector3.zero;
        _revolver.localEulerAngles = Vector3.zero;
    }

    /// <summary>
    /// Change weapon's parent to let it being controlled by armature of the player
    /// </summary>
    public void DeactivateWeapon()
    {
        _revolver.parent = _sheathTransform;
        _revolver.localPosition = Vector3.zero;
        _revolver.localEulerAngles = Vector3.zero;
    }


    /// <summary>
    /// Puts in hand parent so I won't need to animated gun moving from drawn to sheath
    /// </summary>
    public void PutInHand()
    {
        _revolver.parent = _handTransform;
        _revolver.localPosition = Vector3.zero;
        _revolver.localEulerAngles = Vector3.zero;
    }

    public void Shoot()
    {
        if (_drawn)
        {
            if (!_hammered)
            {
                _revolverAnimator.SetTrigger("PrepareForShooting");
                _hammered = true;
            }
            else
            {
                _hitbox = new RaycastHit();

                _revolverAnimator.SetTrigger("Shot2");
                _hammered = false;
            }
        }
    }

    public void RegisterHittingHitbox()
    {
        if (_hitTheHitbox && _hitbox.collider != null)
        {
            if (_hitbox.collider.TryGetComponent(out Prop prop))
            {
                prop.PlayRicochetSound();
                return;
            }

            if (_player)
            {
                if (_hitbox.collider.TryGetComponent(out EnemyHitbox hitbox))
                    _hitbox.collider.GetComponent<EnemyHitbox>().GotShot(_revolver.position);
            }
            else
            {
                if (_hitbox.collider.TryGetComponent(out PlayerHitbox hitbox))
                    _hitbox.collider.GetComponent<PlayerHitbox>().GotShot(_revolver.position);
            }
        }
    }


}
