using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Манекен лучника — атакует ближайшую цель в зоне с кулдауном.
public class ArcherDummy : SingleTargetAttackerMono
{
    [SerializeField] float _damage = 15f;
    [SerializeField] TextMeshPro _totalText;
    [SerializeField] float _dpsWindow = 3f;

    float _totalDamage;
    float _lastHit;

    readonly Queue<(float time, float amount)> _hits = new();

    protected override (IHittable target, Vector3 pos) SelectTarget() => SelectNearest();

    protected override void Attack(IHittable target, Vector3 targetPos)
    {
        target.TakeDamage(_damage);
        _totalDamage += _damage;
        _lastHit = _damage;
        _hits.Enqueue((Time.time, _damage));
    }

    protected override void OnUpdate()
    {
        if (_totalText == null) return;

        float cutoff = Time.time - _dpsWindow;
        while (_hits.Count > 0 && _hits.Peek().time < cutoff)
            _hits.Dequeue();

        float windowDamage = 0f;
        foreach (var h in _hits) windowDamage += h.amount;
        float dps = windowDamage / _dpsWindow;

        _totalText.text = $"Total: {_totalDamage:F0}\nLast:  {_lastHit:F0}\nDPS:   {dps:F1}";
    }
}
