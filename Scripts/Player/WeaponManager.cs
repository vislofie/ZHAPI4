using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponManager : MonoBehaviour
{
    [Header("Set up")]
    [SerializeField]
    private Transform _revolver;
    private Animator _revolverAnimator;

    [SerializeField]
    private RigController _weaponRig;

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

    private void Start()
    {
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
        _weaponRig.EnableRig(false);
        //_revolver.position = _drawnTransform.position;
    }

    /// <summary>
    /// Start decreasing attached to weapon rig weight
    /// </summary>
    public void Sheath()
    {
        Debug.Log("Sheath");
        _drawn = false;
        _weaponRig.DisableRig(false);
        //_revolver.position = _sheathTransform.position;
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
                _revolverAnimator.SetTrigger("Shot2");
                _hammered = false;
            }
        }
    }

    
}
