using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnemyBrain : MonoBehaviour
{
    [Header("Animation and IK")]
    [SerializeField]
    private Transform _headRigParent;
    [SerializeField]
    private Transform _weaponRigParent;
    [SerializeField]
    private RigBuilder _rigBuilder;
    [SerializeField][Tooltip("How close can enemy get to target before it stops")]
    private float _closestDistanceToTarget = 5.0f;
    [SerializeField][Tooltip("How much seconds is between shots")]
    private float _shootRate = 2.0f;

    private float _shootCountTime = 0.0f;

    private AIStats _stats;
    private EnemyMovement _movement;
    private EnemySenses _senses;
    private EnemyAnimation _animationController;
    private Rigidbody[] _ragdollBodies;

    private EnemyWeaponManager _weaponManager;

    private Transform _target;

    private bool _weaponDrawn;
    private bool _weaponSheathed;

    private bool _blockedMovement;

    private float _lerpingMovement;

    private void Start()
    {
        _stats = new AIStats();
        _movement = GetComponent<EnemyMovement>();
        _senses = GetComponent<EnemySenses>();
        _animationController = GetComponent<EnemyAnimation>();
        _weaponManager = GetComponent<EnemyWeaponManager>();
        _weaponManager.Initialize(null);

        _blockedMovement = false;

        _ragdollBodies = GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();

        _animationController.Initialize(_headRigParent, _weaponRigParent, _rigBuilder);

        _shootCountTime = _shootRate;
    }

    private void Update()
    {
        if (!_stats.Dead && !_blockedMovement)
        {
            ActUponTarget();
            _senses.FindTargets();
        }
    }

    public void ActUponTarget()
    {
        if (_senses.ClosestTargetTransform != null || _target != null) // if there is a target visible
        {
            _target =_senses.ClosestTargetTransform == null ? (_target.GetComponent<PlayerBrain>().Alive ? _target : null) : _senses.ClosestTargetTransform;

            if (_target != null)
            {
                Transform aimTarget = _target.GetComponent<Target>().AimTarget;

                _stats.SetState(AIState.Hostile);

                Vector3 desiredPos = _target.position - (_target.position - transform.position).normalized * _closestDistanceToTarget;

                if (Vector3.Angle(transform.position - _target.position, transform.position - desiredPos) > 1 || transform.position == desiredPos)
                {
                    _movement.StopMovement();
                    _movement.LookAt(_target.position);
                    _lerpingMovement = Mathf.Lerp(_lerpingMovement, 0.0f, 5 * Time.deltaTime);
                    _animationController.SetMovement(_lerpingMovement, 0.0f);
                }
                else
                {
                    _movement.MoveToPosition(desiredPos);
                    _movement.LookAt(desiredPos);
                    _lerpingMovement = Mathf.Lerp(_lerpingMovement, 1.0f, 5 * Time.deltaTime);
                    _animationController.SetMovement(_lerpingMovement, 0.0f);
                }


                if (!_weaponDrawn)
                {
                    _animationController.DrawWeapon();
                    _weaponDrawn = true;
                    _weaponSheathed = false;
                }

                if (_shootCountTime >= _shootRate - Random.Range(0.0f, 0.5f))
                {
                    _weaponManager.Shoot();
                    _shootCountTime = 0.0f;
                }
                _weaponManager.AimAndShoot(aimTarget);

                if (!_animationController.HeadRigTargetSet)
                    _animationController.SetTargetHeadRig(aimTarget);
                if (!_animationController.WeaponRigTargetSet && _weaponManager.Drawn)
                    _animationController.SetWeaponRig(aimTarget);

                _shootCountTime += Time.deltaTime;
            }
            else
            {
                if (_stats.CurrentState == AIState.Hostile)
                {
                    if (_movement.ReachedTheDestination())
                    {
                        _movement.Idle();
                        _stats.SetState(AIState.Idle);
                        _animationController.SetMovement(0.0f, 0.0f);

                        if (!_weaponSheathed)
                        {
                            _animationController.SheathWeapon();
                            _weaponDrawn = false;
                            _weaponSheathed = true;
                        }

                        if (_animationController.HeadRigTargetSet)
                            _animationController.ResetTargetHeadRig();

                        if (_animationController.WeaponRigTargetSet && !_weaponManager.Drawn)
                            _animationController.ResetWeaponRig();
                    }
                }

                _shootCountTime = 0.0f;
            }
        } 
    }
    public void DetectShot(float damage)
    {
        if (_target == null && damage < 100.0f)
        {
            _stats.ReduceHP(damage);

            GameObject possibleTarget = GameObject.FindGameObjectWithTag("Player");

            if (possibleTarget.layer == LayerMask.NameToLayer("Player"))
                _target = possibleTarget.transform;
        }
            

        _stats.ReduceHP(damage);

        if (_ragdollBodies != null && _stats.HP == 0.0f && !_stats.Dead)
        {
            EnableRagdoll();
            _movement.Idle();
            _animationController.Deactivate();
            _stats.Kill();

            GameManager.EnemyOut();

            if (GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Stop();
        }
    }

    public void ShootOffInDirection(Vector3 direction)
    {
        foreach (Rigidbody body in _ragdollBodies)
        {
            body.AddForce(direction);
        }
    }

    public void Pause()
    {
        GetComponent<AudioSource>().Pause();
        _blockedMovement = true;
    }

    public void UnPause()
    {
        GetComponent<AudioSource>().UnPause();
        _blockedMovement = false;
    }

    private void DisableRagdoll()
    {
        foreach (Rigidbody body in _ragdollBodies)
        {
            body.isKinematic = true;
        }
    }

    private void EnableRagdoll()
    {
        foreach (Rigidbody body in _ragdollBodies)
        {
            body.isKinematic = false;
            body.gameObject.layer = 11;
        }
    }
}
