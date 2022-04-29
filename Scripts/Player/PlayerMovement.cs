using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerMovement: MonoBehaviour
{
    [Header("IK")]
    [SerializeField]
    private RigController _headRig;
    [SerializeField]
    private Transform _targetTransform;
    [Tooltip("Maximum distance to target to disable IK")]
    [SerializeField]
    private float _maxDistanceToTarget = 5.0f;

    [Header("Movement")]
    [SerializeField]
    private float _walkingSpeed = 5.0f;
    [SerializeField]
    private float _runningSpeed = 12.0f;

    
    private float _forwardSpeed = 0.0f; // varies from 0 to 1
    private float _lateralSpeed = 0.0f; // varies from -1 to 1

    private WeaponManager _weaponManager;

    public float ForwardSpeed => _forwardSpeed;
    public float LateralSpeed => _lateralSpeed;

    private bool _running;
    private bool _aiming;

    private CameraController _playerCamera;
    private Animator _animator;

    private void Awake()
    {
        _playerCamera = Camera.main.GetComponent<CameraController>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _weaponManager = GetComponent<WeaponManager>();
        _running = false;
        _aiming = false;
    }

    public void UpdateMovement()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _aiming = !_aiming;
            _playerCamera.AimMode(_aiming);

            if (_aiming)
            {
                _animator.SetTrigger("draw");
                _targetTransform.gameObject.SetActive(true);

                _running = false;
                _playerCamera.StopRunning();
            }
            else
            {
                _animator.SetTrigger("sheath");
                _targetTransform.gameObject.SetActive(false);
            }
        }

        else if (Input.GetMouseButtonDown(0))
        {
            if (_aiming)
                _weaponManager.Shoot();
        }

        ChangePositionOnInput();
        IKAndAnimation();

        _targetTransform.position = transform.position + _playerCamera.transform.forward * (_maxDistanceToTarget-2) + new Vector3(0, 1.5f, 0);
    }

    private void ChangePositionOnInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !_aiming)
        {
            _running = true;
            _playerCamera.StartRunning();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && !_aiming)
        {
            _running = false;
            _playerCamera.StopRunning();
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        
        if (_running && !_aiming)
        {
            _forwardSpeed = Mathf.Lerp(_forwardSpeed, vertical, 10 * Time.deltaTime);
            _lateralSpeed = Mathf.Lerp(_lateralSpeed, horizontal, 10 * Time.deltaTime);
        }
        else
        {
            _forwardSpeed = Mathf.Lerp(_forwardSpeed, vertical / 2, 10 * Time.deltaTime);
            _lateralSpeed = Mathf.Lerp(_lateralSpeed, horizontal / 2, 10 * Time.deltaTime);
        }
        

        if ((Mathf.Abs(horizontal) >= 0.01f || Mathf.Abs(vertical) >= 0.01f) || _aiming)
            transform.localEulerAngles = new Vector3(0, _playerCamera.transform.eulerAngles.y, 0);

        Vector3 movementVector = new Vector3(horizontal, 0, vertical).normalized;

        if (vertical >= 0.0f) transform.Translate(movementVector * ((_running && !_aiming) ? _runningSpeed : _walkingSpeed) * Time.deltaTime);
        else transform.Translate(movementVector * ((_running && !_aiming) ? _runningSpeed : _walkingSpeed) * Time.deltaTime / 2);


    }

    private void IKAndAnimation()
    {
        if (Vector3.Distance(transform.position, _targetTransform.position) > _maxDistanceToTarget)
        {
            if (_headRig.Enabled)
                _headRig.DisableRig(true);
        }
        else
        {
            if (!_headRig.Enabled)
                _headRig.EnableRig(true);
        }

        _animator.SetFloat("forwardSpeed", _forwardSpeed);
        _animator.SetFloat("lateralSpeed", _lateralSpeed);
    }

}
