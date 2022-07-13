using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private PlayerBrain _attachedBrain;

    private void Start()
    {
        _attachedBrain = GetComponentInParent<PlayerBrain>();
    }
    public void GotShot(Vector3 origin)
    {
        _attachedBrain.DetectShot(7.5f);
    }
}
