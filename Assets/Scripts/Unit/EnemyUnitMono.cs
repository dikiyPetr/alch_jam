using System;
using UnityEngine;

// Базовый юнит врага: хранит HP, принимает урон, умирает.
public class EnemyUnitMono : UnitMono, IHittable
{
    [SerializeField] float _maxHp = 100f;
    [SerializeField] float _moveSpeed = 3f;

    float _currentHp;

    public event Action OnDied;

    protected override float MoveSpeed => _moveSpeed;

    protected override void Awake()
    {
        base.Awake();
        _currentHp = _maxHp;
    }

    public void TakeDamage(float amount)
    {
        if (_currentHp <= 0f) return;
        _currentHp -= amount;
        if (_currentHp <= 0f)
            Die();
    }

    void Die()
    {
        OnDied?.Invoke();
        Destroy(gameObject);
    }
}
