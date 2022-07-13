using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    [SerializeField]
    private Transform _targetTransform;
    [SerializeField][Tooltip("Material of the target aim that indicates that player isn't aiming into anything")]
    private Material _normalAimMaterial;
    [SerializeField][Tooltip("Material of the target aim that indicates that player is aiming at enemy or any other hostile objects")]
    private Material _enemyAimMaterial;

    private PlayerMovement _movement;
    private PlayerWeaponManager _weaponManager;
    private PlayerStats _playerStats;
    private PlayerUI _playerUI;

    private CameraController _cameraController;

    private Rigidbody[] _ragdollBodies;
    private Collider[] _ragdollColliders;
    private Rigidbody _entityRigidbody;
    private Collider _entityCollider;

    private Animator _animator;

    private bool _alive;
    public bool Alive => _alive;

    private bool _allowedToMove;

    private bool _countingTime;

    private void Awake()
    {
        _alive = true;
        _countingTime = false;

        _movement = GetComponent<PlayerMovement>();
        _weaponManager = GetComponent<PlayerWeaponManager>();
        _playerStats = GetComponent<PlayerStats>();
        _playerUI = GetComponent<PlayerUI>();

        _cameraController = Camera.main.GetComponent<CameraController>();

        _ragdollBodies = GetComponentsInChildren<Rigidbody>();
        _ragdollColliders = GetComponentsInChildren<Collider>();
        _entityRigidbody = GetComponent<Rigidbody>();
        _entityCollider = GetComponent<Collider>();

        _animator = GetComponentInChildren<Animator>();

        _movement.Initialize(_targetTransform);
        _weaponManager.Initialize(_targetTransform, _normalAimMaterial, _enemyAimMaterial);
        _playerStats.Initialize(100.0f);
        _playerUI.UpdateHealth(100.0f);
    }

    private void Start()
    {
        DisableRagdoll();

        //PlayerPrefs.SetInt("SeenCutscene", 0);

        _playerUI.LockCursor();

        if (PlayerPrefs.GetInt("SeenCutscene", 0) == 0)
        {
            PlayerPrefs.SetInt("SeenCutscene", 1);
        }
        else
        {
            _cameraController.gameObject.GetComponent<AudioSource>().time = 25.2f;
            _cameraController.StopCutscene();

            _animator.SetTrigger("cutWalking");

            PreCutsceneEnd();
            CutsceneEnd();
        }
    }

    private void Update()
    {
        if (_allowedToMove)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenMenu();
            }
            if (_alive)
            {
                _movement.UpdateMovement();

                if (_countingTime)
                    _playerUI.UpdateTimer(_playerStats.SecondsPassed);
            }
        }
    }

    public void DetectShot(float damage)
    {
        _playerStats.ReduceHP(Random.Range(damage - damage / 2, damage + damage / 2));
        _playerUI.UpdateHealth(_playerStats.HP);
    }

    public void EnableRagdoll()
    {
        for (int i = 0; i < _ragdollBodies.Length; i++)
        {
            _ragdollBodies[i].isKinematic = false;
            _ragdollColliders[i].enabled = true;
        }

        _entityRigidbody.isKinematic = true;
        _entityCollider.enabled = false;

        _animator.enabled = false;
    }

    public void DisableRagdoll()
    {
        for (int i = 0; i < _ragdollBodies.Length; i++)
        {
            _ragdollBodies[i].isKinematic = true;
            _ragdollColliders[i].enabled = false;
        }

        _entityRigidbody.isKinematic = false;
        _entityCollider.enabled = true;

        _animator.enabled = true;
    }

    public void ShootOffInDirection(Vector3 direction)
    {
        foreach (Rigidbody body in _ragdollBodies)
        {
            body.AddForce(direction);
        }
    }

    public void Die()
    {
        _alive = false;
        gameObject.layer = 0;
        EnableRagdoll();

        _targetTransform.gameObject.SetActive(false);

        _playerUI.GameOver();
        _playerUI.UnlockCursor();
        _cameraController.BlockMovement();
    }
    
    public void BlockMovement()
    {
        _allowedToMove = false;
    }

    public void AllowMovement()
    {
        _allowedToMove = true;
    }

    public void BlockCameraMovement()
    {
        _cameraController.BlockMovement();
    }

    public void AllowCameraMovement()
    {
        _cameraController.AllowMovement();
    }

    public void FinishLevel()
    {
        _countingTime = false;

        int secondsPassed = Mathf.FloorToInt(_playerStats.SecondsPassed);
        int savedSeconds = PlayerPrefs.GetInt("TimeScore", -1);

        if (savedSeconds == -1 || savedSeconds > secondsPassed)
            PlayerPrefs.SetInt("TimeScore", Mathf.FloorToInt(_playerStats.SecondsPassed));

        _playerUI.LevelFinish();
        _playerUI.UnlockCursor();
        _cameraController.BlockMovement();
        BlockMovement();
    }

    public void Resume()
    {
        _playerUI.LockCursor();
        _cameraController.AllowMovement();
        AllowMovement();
    }

    public void PreCutsceneEnd()
    {
        _animator.SetTrigger("stopWalking");
    }

    public void CutsceneEnd()
    {
        Transform playerModel = transform.GetChild(0);

        transform.position = playerModel.position;
        transform.rotation = playerModel.rotation;

        playerModel.localPosition = Vector3.zero;
        playerModel.localRotation = Quaternion.identity;

        _allowedToMove = true;

        _playerUI.ActivateUI();

        _playerStats.StartTheClock();

        _countingTime = true;

        GameManager.CutSound();
    }

    public void OpenMenu()
    {
        _playerUI.OpenMenu();
        Pause();
    }

    public void Pause()
    {
        GameManager.Pause();
    }

    public void UnPause()
    {
        GameManager.UnPause();
    }
}
