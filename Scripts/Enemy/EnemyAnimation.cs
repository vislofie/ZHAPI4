using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnemyAnimation : MonoBehaviour
{
    private float _forwardSpeed;
    private float _lateralSpeed;

    private Animator _animator;

    private MultiAimConstraint[] _headConstraints;
    private MultiAimConstraint _weaponConstraint;

    private RigBuilder _rigBuilder;

    private bool _headRigTargetSet;
    public bool HeadRigTargetSet => _headRigTargetSet;

    private bool _weaponRigTargetSet;
    public bool WeaponRigTargetSet => _weaponRigTargetSet;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void Initialize(Transform headRigParent, Transform weaponRigParent, RigBuilder rigBuilder)
    {
        _headConstraints = headRigParent.GetComponentsInChildren<MultiAimConstraint>();
        _weaponConstraint = weaponRigParent.GetComponentInChildren<MultiAimConstraint>();

        _rigBuilder = rigBuilder;
        _headRigTargetSet = false;
    }

    public void Deactivate()
    {
        _animator.enabled = false;

        foreach (RigLayer layer in _rigBuilder.layers)
        {
            layer.rig.weight = 0.0f;
        }
    }

    public void SetTargetHeadRig(Transform target)
    {
        if (_headConstraints.Length > 0)
        {
            foreach (MultiAimConstraint constraint in _headConstraints)
            {
                WeightedTransformArray sourceData = constraint.data.sourceObjects;

                sourceData.Clear();
                sourceData.Add(new WeightedTransform(target, 1));

                constraint.data.sourceObjects = sourceData;
            }

            _rigBuilder.Build();

            _headRigTargetSet = true;
        }
    }

    public void SetWeaponRig(Transform target)
    {
        if (_weaponConstraint != null)
        {
            WeightedTransformArray sourceData = _weaponConstraint.data.sourceObjects;

            sourceData.Clear();
            sourceData.Add(new WeightedTransform(target, 1));

            _weaponConstraint.data.sourceObjects = sourceData;

            _rigBuilder.Build();

            _weaponRigTargetSet = true;
        }
    }

    public void ResetTargetHeadRig()
    {
        if (_headConstraints.Length > 0)
        {
            foreach (MultiAimConstraint constraint in _headConstraints)
            {
                WeightedTransformArray sourceData = constraint.data.sourceObjects;

                sourceData.Clear();

                constraint.data.sourceObjects = sourceData;
            }

            _rigBuilder.Build();

            _headRigTargetSet = false;
        }
    }

    public void ResetWeaponRig()
    {
        if (_weaponConstraint != null)
        {
            WeightedTransformArray sourceData = _weaponConstraint.data.sourceObjects;

            sourceData.Clear();

            _weaponConstraint.data.sourceObjects = sourceData;

            _rigBuilder.Build();

            _weaponRigTargetSet = false;
        }
    }

    public void DrawWeapon()
    {
        _animator.SetTrigger("draw");
    }

    public void SheathWeapon()
    {
        _animator.SetTrigger("sheath");
    }

    public void SetMovement(float forwardSpeed, float lateralSpeed)
    {
        _forwardSpeed = forwardSpeed;
        _lateralSpeed = lateralSpeed;

        _animator.SetFloat("forwardSpeed", _forwardSpeed);
        _animator.SetFloat("lateralSpeed", _lateralSpeed);
    }
}
