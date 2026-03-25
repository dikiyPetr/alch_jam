using System;
using UnityEngine;

// Снаряд с уроном по области — движется к цели и существует _lifetime секунд.
// Вешается на префаб с Collider-триггером. Запускается через Launch().
public class ProjectileAreaDamage : AreaDamageMono
{
    [SerializeField] float _damage = 20f;
    [SerializeField] float _speed = 8f;
    [SerializeField] float _lifetime = 3f;
    [SerializeField] float _arrivalRadius = 0.3f;

    // Вызывается каждый тик с суммарным уроном по всем целям
    public event Action<float> OnDamageDealt;

    Vector3 _targetPos;
    bool _launched;

    protected override void OnAwake()
    {
        Destroy(gameObject, _lifetime);
    }

    public void Launch(Vector3 targetPos)
    {
        _targetPos = targetPos;
        _launched = true;
    }

    protected override void OnUpdate()
    {
        if (!_launched) return;
        var dir = _targetPos - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude <= _arrivalRadius * _arrivalRadius)
        {
            _launched = false;
            return;
        }

        transform.position += dir.normalized * (_speed * Time.deltaTime);
    }

    protected override void Tick()
    {
        if (_rangeFinder == null || _rangeFinder.InRange.Count == 0) return;
        _rangeFinder.RemoveDeadEntries();
        float dmg = GetDamage();
        float total = dmg * _rangeFinder.InRange.Count;
        foreach (var target in _rangeFinder.InRange)
            target.TakeDamage(dmg);
        if (total > 0f) OnDamageDealt?.Invoke(total);
    }

    protected override float GetDamage() => _damage;
}