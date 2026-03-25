using System.Collections.Generic;
using UnityEngine;

// Специализация DamageSourceMono для периодического урона в зоне коллизии.
// Отслеживает IHittable-цели внутри триггера и наносит урон каждый тик.
// Коллайдер должен быть триггером — Awake выставляет isTrigger автоматически.
[RequireComponent(typeof(Collider))]
public abstract class AreaDamageMono : DamageSourceMono
{
    [SerializeField] UnitMono _source;

    readonly HashSet<IHittable> _inRange = new();
    [SerializeField] protected Collider _collider;

    protected virtual void Awake()
    {
        _collider.isTrigger = true;
    }

    protected override void Tick()
    {
        if (_inRange.Count == 0) return;
        float dmg = GetDamage();
        foreach (var target in _inRange)
            target.TakeDamage(dmg, _source);
    }

    void OnTriggerEnter(Collider other)
    {
        if (TryGetTarget(other.gameObject, out var target))
            _inRange.Add(target);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IHittable>(out var target))
            _inRange.Remove(target);
    }

    void OnDrawGizmos()
    {
        if (_collider == null) return;
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.25f);
        Gizmos.matrix = _collider.transform.localToWorldMatrix;

        switch (_collider)
        {
            case SphereCollider s:
                Gizmos.DrawSphere(s.center, s.radius);
                Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
                Gizmos.DrawWireSphere(s.center, s.radius);
                break;
            case BoxCollider b:
                Gizmos.DrawCube(b.center, b.size);
                Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
                Gizmos.DrawWireCube(b.center, b.size);
                break;
            case CapsuleCollider c:
                // Капсула через две сферы + цилиндр — приближение Wire-сферами
                float r = c.radius;
                float halfH = Mathf.Max(0f, c.height * 0.5f - r);
                var top    = c.center + Vector3.up * halfH;
                var bottom = c.center - Vector3.up * halfH;
                Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
                Gizmos.DrawWireSphere(top, r);
                Gizmos.DrawWireSphere(bottom, r);
                break;
        }
    }
}
