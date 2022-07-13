using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigController : MonoBehaviour
{
    public bool Enabled { get; private set; }
    private Rig _rig;

    private void Start()
    {
        _rig = GetComponent<Rig>();
    }

    public void DisableRig(bool gradually, RigBuilder rigBuilder = null)
    {
        Enabled = false;
        StopAllCoroutines();
        StartCoroutine(LerpWeight(true, gradually, rigBuilder));

    }

    public void EnableRig(bool gradually, RigBuilder rigBuilder = null)
    {
        Enabled = true;
        StopAllCoroutines();
        StartCoroutine(LerpWeight(false, gradually, rigBuilder));
    }

    private IEnumerator LerpWeight(bool disable, bool gradually, RigBuilder rigBuilder)
    {
        float additiveValue = gradually ? 1f : 15f;
        
        yield return new WaitForEndOfFrame();

        _rig.weight = disable ? 0.99f : 0.01f;
        while (_rig.weight > 0.0f && _rig.weight < 1.0f)
        {
            _rig.weight = Mathf.Clamp(_rig.weight + (disable ? -additiveValue * Time.deltaTime : additiveValue * Time.deltaTime), 0.0f, 1.0f);

            if (_rig.weight > 0.0f && _rig.weight < 1.0f)
                if (rigBuilder != null)
                    rigBuilder.Build();
                yield return new WaitForEndOfFrame();
        }

        
    }
}
