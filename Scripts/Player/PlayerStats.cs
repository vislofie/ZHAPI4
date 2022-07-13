using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private PlayerBrain _brain;

    private float _hp;
    public float HP => _hp;

    private float _secondsPassed;
    public float SecondsPassed => _secondsPassed;

    private void Update()
    {
        if (_secondsPassed >= 0.0f)
            _secondsPassed += Time.deltaTime;
    }

    public void Initialize(float startHp)
    {
        _brain = GetComponent<PlayerBrain>();
        _hp = startHp;
        _secondsPassed = -1.0f;
    }

    public void ReduceHP(float hp)
    {
        if (hp > 0)
        {
            _hp -= hp;

            if (_hp < 0.0f)
            {
                _hp = 0.0f;
                _brain.Die();
            }
            
        }
    }

    public void StartTheClock()
    {
        _secondsPassed = 0.0f;
    }
}
