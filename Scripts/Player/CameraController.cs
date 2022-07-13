using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Control")]
    [Header("Aiming")]
    
    [SerializeField]
    private Vector3 _noAimOffset;
    [SerializeField]
    private Vector3 _aimOffset;

    private Vector3 _offset;

    [SerializeField]
    private float _noAimDistanceFromTarget = 3.0f;
    [SerializeField]
    private float _aimDistanceFromTarget = 1.5f;

    private float _distanceFromTarget;

    [Header("")]

    [SerializeField]
    private float _mouseSensitivity = 3.0f;

    private float _rotationY;
    private float _rotationX;

    private Transform _target;

    private Vector3 _currentPosition;
    private Vector3 _currentRotation;
    private Vector3 _smoothVelocity = Vector3.zero;

    [SerializeField]
    private bool _smoothing = false;

    [SerializeField]
    private float _smoothTime = 0.1f;

    [SerializeField]
    private Vector2 _rotationAimXMinMax = new Vector2(-40, 90);
    [SerializeField]
    private Vector2 _rotationNoAimXMinMax = new Vector2(-70, 70);
    private Vector2 _rotationXMinMax;

    [Header("Camera Anti-Clip")]
    [Tooltip("Layers that are going to be interacted with while looking for obstacles for camera")]
    [SerializeField]
    private LayerMask _layersToCollide;

    [SerializeField]
    private Vector3 _targetPivotOffset;

    [Header("Camera Bob")]
    [SerializeField]
    private float _bobFrequency = 0.1f;
    [SerializeField]
    private float _bobAmplitude = 1.0f;

    private float _bobTime = 0.0f;

    private bool _blockedMovement;

    private bool _running = false;
    private bool _bobFinished = false;

    private void Awake()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        AimMode(false);
        _blockedMovement = true;
        _target.GetComponent<PlayerBrain>().BlockMovement();
    }

    private void Update()
    {
        if (!_blockedMovement)
        {
            OrbitCamera();

            if (_running)
                CameraBob();
            else if (!_running && !_bobFinished)
                FinishBob();

            MoveByCollision();

            ApplyPosChange();
        }
    }

    public void StartRunning()
    {
        _running = true;
    }

    public void StopRunning()
    {
        _running = false;
    }
    /// <summary>
    /// Changes camera to aim distance from player
    /// </summary>
    /// <param name="enabled"></param>
    public void AimMode(bool enabled)
    {
        if (enabled)
        {
            _distanceFromTarget = _aimDistanceFromTarget;
            _offset = _aimOffset;
            _rotationXMinMax = _rotationAimXMinMax;
        }
        else
        {
            _distanceFromTarget = _noAimDistanceFromTarget;
            _offset = _noAimOffset;
            _rotationXMinMax = _rotationNoAimXMinMax;
        }
        
    }
    /// <summary>
    /// Orbiting the camera around the target using mouse input
    /// </summary>
    private void OrbitCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        _rotationY += mouseX;
        _rotationX -= mouseY;

        // Apply clamping for x rotation 
        _rotationX = Mathf.Clamp(_rotationX, _rotationXMinMax.x, _rotationXMinMax.y);

        Vector3 nextRotation = new Vector3(_rotationX, _rotationY);

        // Apply damping between rotation changes
        _currentRotation = _smoothing ? Vector3.SmoothDamp(_currentRotation, nextRotation, ref _smoothVelocity, _smoothTime) : nextRotation;
        transform.localEulerAngles = _currentRotation;

        // Substract forward vector of the GameObject to point its forward vector to the target
        _currentPosition = _target.position - transform.forward * _distanceFromTarget;
    }

    /// <summary>
    /// Moves camera considering collisions with objects
    /// </summary>
    private void MoveByCollision()
    {
        RaycastHit hit;
        if (Physics.Linecast(_target.position + _targetPivotOffset, _currentPosition, out hit, _layersToCollide))
        {
            _currentPosition = hit.point;
        }
    }

    /// <summary>
    /// Applies differences to positions that are made before calling this method
    /// </summary>
    private void ApplyPosChange()
    {
        Vector3 aimOffset = Vector3.Cross(-transform.forward, transform.up) * _offset.z;
        Vector3 topOffset = Vector3.Cross(transform.forward, transform.right) * _offset.y;
        transform.position = _currentPosition + aimOffset + topOffset;
    }

    /// <summary>
    /// Bobbing the camera using Sin function
    /// </summary>
    private void CameraBob()
    {
        _currentPosition += new Vector3(0.0f, Mathf.Sin(_bobTime * _bobFrequency) * _bobAmplitude, 0.0f);
        _bobFinished = false;
        _bobTime += Time.deltaTime;
    }

    /// <summary>
    /// Finishing Bobbing so it wont look ugly
    /// </summary>
    private void FinishBob()
    {
        _bobFinished = true;
        _bobTime = 0.0f;
    }
    public void PreStopCutscene()
    {
        _target.GetComponent<PlayerBrain>().PreCutsceneEnd();
    }

    public void StopCutscene()
    {
        GetComponent<Animation>().enabled = false;
        _blockedMovement = false;

        _target.GetComponent<PlayerBrain>().CutsceneEnd();

        _currentRotation = transform.rotation.eulerAngles;

        _rotationY = transform.rotation.eulerAngles.y;
        _rotationX = transform.rotation.eulerAngles.x;
    }

    public void SetSensitivity(float value)
    {
        _mouseSensitivity = value;
    }

    public void BlockMovement()
    {
        _blockedMovement = true;
    }

    public void AllowMovement()
    {
        _blockedMovement = false;
    }
}