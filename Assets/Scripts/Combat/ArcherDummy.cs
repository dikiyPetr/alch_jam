using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Манекен лучника — стреляет стрелой в ближайшую цель.
public class ArcherDummy : SingleTargetAttackerMono
{
    [SerializeField] ArrowProjectile _arrowPrefab;
    [SerializeField] float _damage = 15f;
    [SerializeField] float _arrowSpeed = 12f;
    [SerializeField] TextMeshPro _totalText;
    [SerializeField] float _dpsWindow = 3f;

    float _totalDamage;
    float _lastHit;

    readonly Queue<(float time, float amount)> _hits = new();

    protected override (IHittable target, Vector3 pos) SelectTarget() => SelectNearest();

    protected override void Attack(IHittable target, Vector3 targetPos)
    {
        if (target is not Component c) return;
        var arrow = Instantiate(_arrowPrefab, transform.position, Quaternion.identity);
        arrow.Launch(c.transform, _damage, _arrowSpeed);
        arrow.OnHit += amount =>
        {
            _totalDamage += amount;
            _lastHit = amount;
            _hits.Enqueue((Time.time, amount));
        };
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
