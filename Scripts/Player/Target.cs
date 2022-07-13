using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField]
    private Transform _aimTarget;

    public Transform AimTarget => _aimTarget;
}
