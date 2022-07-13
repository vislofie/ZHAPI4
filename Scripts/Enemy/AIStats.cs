public enum AIState { Idle, Patroling, Hostile };
public class AIStats
{
    private AIState _currentState;
    private float _hp;
    private float _hpMax;

    private bool _dead;

    public float HP => _hp;
    public bool Dead => _dead;
    public AIState CurrentState => _currentState;

    public AIStats()
    {
        _hp = 100.0f;
        _hpMax = 100.0f;
        _dead = false;
        _currentState = AIState.Idle;
    }
    public AIStats(AIState state, float hp, float hpMax = 100.0f)
    {
        _hp = hp;
        _hpMax = hpMax;
        _currentState = state;
    }

    public void ReduceHP(float amount)
    {
        _hp = _hp - amount > 0.0f ? _hp - amount : 0.0f;
    }

    public void AddHP(float amount)
    {
        _hp = _hp + amount <= _hpMax ? _hp + amount : _hpMax;
    }

    public void Kill()
    {
        _dead = true;
    }

    public void Revive()
    {
        _dead = false;
    }

    public void SetState(AIState state)
    {
        _currentState = state;
    }
}
