using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySenses : MonoBehaviour
{
    [SerializeField]
    private float _viewAngle;
    [SerializeField]
    private float _viewRadius;

    Vector3 _targetPosition;
    Vector3 _ownPosition;

    public float ViewAngle => _viewAngle;
    public float ViewRadius => _viewRadius;

    [SerializeField]
    private LayerMask _targetMask;
    private List<Transform> _visibleTargets = new List<Transform>();

    public Transform ClosestTargetTransform
    {
        get
        {
            float minDistance = float.MaxValue;
            Transform closestTarget = null;
            foreach (Transform target in _visibleTargets)
            {
                float distance = Vector3.Distance(transform.position, target.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }
    }

    public void FindTargets()
    {

        Collider[] colliders = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);

        for (int i = _visibleTargets.Count - 1; i >= 0; i--)
        {
            if (Vector3.Distance(_visibleTargets[i].position, transform.position) > _viewRadius || ((int)Mathf.Pow(2, _visibleTargets[i].gameObject.layer) & _targetMask.value) == 0)
            {
                _visibleTargets.RemoveAt(i);
            }
        }

        foreach (Collider collider in colliders)
        {
            Transform targetTransform = collider.transform;
            _targetPosition = targetTransform.position + Vector3.up / 2;
            _ownPosition = transform.position + Vector3.up / 2;

            Vector3 dirToTarget = (_targetPosition - _ownPosition).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < _viewAngle / 2)
            {
                float distance = Vector3.Distance(_ownPosition, _targetPosition);
                RaycastHit hit;
                if (Physics.Raycast(_ownPosition, dirToTarget, out hit, distance, _targetMask))
                {
                    if (hit.collider == collider && !_visibleTargets.Contains(targetTransform) && hit.collider.tag == "Player")
                        _visibleTargets.Add(targetTransform);
                }
            }
        }
    }

    public void AddTarget(Transform target)
    {
        if (!_visibleTargets.Contains(target))
            _visibleTargets.Add(target);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
            angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
