using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Манекен для нанесения урона — периодически бьёт всё в зоне _rangeFinder.
public class DamageDealerDummy : AreaDamageMono
{
    [SerializeField] float _damage = 10f;
    [SerializeField] TextMeshPro _totalText;
    [SerializeField] float _dpsWindow = 3f;

    float _totalDamage;
    float _lastHit;

    readonly Queue<(float time, float amount)> _hits = new();

    protected override float GetDamage() => _damage;

    protected override void Tick()
    {
        if (_rangeFinder == null || _rangeFinder.InRange.Count == 0) return;
        _rangeFinder.RemoveDeadEntries();
        float dmg = GetDamage();
        float tickTotal = dmg * _rangeFinder.InRange.Count;
        _totalDamage += tickTotal;
        _lastHit = tickTotal;
        _hits.Enqueue((Time.time, tickTotal));
        foreach (var target in _rangeFinder.InRange)
            target.TakeDamage(dmg);
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

        _totalText.text = $"Total: {_totalDamage:F0}\nTargets: {_rangeFinder?.InRange.Count ?? 0}\nDPS:   {dps:F1}";
    }
}
