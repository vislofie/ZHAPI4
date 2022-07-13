using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{

    private NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void MoveToPosition(Vector3 position)
    {
        _agent.speed = 1.0f;
        _agent.SetDestination(position);
    }

    public void StopMovement()
    {
        _agent.speed = 0.0f;
    }

    public void LookAt(Vector3 position)
    {
        transform.LookAt(position);
    }

    public bool ReachedTheDestination()
    {
        return _agent.remainingDistance < 1.0f;
    }

    public void Idle()
    {
        _agent.isStopped = true;
    }
}
