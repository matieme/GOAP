using UnityEngine;

public class EntityLife : MonoBehaviour
{
    public float Life
    {
        get { return _life; }
        set
        {
            _life = value;
            if (_life > _lifeMax) _life = _lifeMax;
            else if (_life < 0) _life = 0;
        }
    }

    [SerializeField] protected float _life;
    public int battery;
    protected float _lifeMax;
    protected bool _isDeath;

    public virtual void Awake()
    {
        _lifeMax = _life;
    }

    public virtual void Damaged(float damage)
    {
        if (_isDeath) return;

        Life -= damage;
        if (Life <= 0) Destroyed();
    }

    public virtual void Healed(float ammountHealed)
    {
        Life += ammountHealed;
    }

    public virtual void ReplenishBattery(int batteryChargeAmount)
    {
        battery += batteryChargeAmount;
    }

    public virtual void Destroyed()
    {
        if (_isDeath) return;
        _isDeath = true;
    }
}
