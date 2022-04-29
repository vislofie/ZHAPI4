using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnimChecker : MonoBehaviour
{
    [SerializeField]
    private AnimationClip[] _animationsToCheck;

    private void Start()
    {
        foreach (AnimationClip clip in _animationsToCheck)
        {
            Debug.Log(clip.name + " is " + (clip.humanMotion ? "Humanoid" : "Not humanoid"));
        }
    }
}
