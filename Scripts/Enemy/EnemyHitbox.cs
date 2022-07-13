using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private EnemyBrain _attachedBrain;

    private void Start()
    {
        _attachedBrain = GetComponentInParent<EnemyBrain>();
    }
    public void GotShot(Vector3 origin)
    {
        _attachedBrain.DetectShot(50.0f);
        GetComponent<Rigidbody>().AddForce((transform.position - origin).normalized * 100.0f);
    }
}
