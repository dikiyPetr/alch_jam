using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DummyMono : MonoBehaviour, IHittable
{
    [SerializeField] TextMeshPro _totalText;

    [SerializeField] float _dpsWindow = 3f;

    float _totalDamage;
    float _lastHit;

    // Очередь пар (время удара, урон) для скользящего DPS-окна
    readonly Queue<(float time, float amount)> _hits = new();

    public void TakeDamage(float amount, UnitMono source)
    {
        _totalDamage += amount;
        _lastHit = amount;
        _hits.Enqueue((Time.time, amount));
    }

    void Update()
    {
        // Удаляем удары старше окна
        float cutoff = Time.time - _dpsWindow;
        while (_hits.Count > 0 && _hits.Peek().time < cutoff)
            _hits.Dequeue();

        float windowDamage = 0f;
        foreach (var h in _hits) windowDamage += h.amount;
        float dps = windowDamage / _dpsWindow;

        _totalText.text = $"Total: {_totalDamage:F0}\nLast:  {_lastHit:F0}\nDPS:   {dps:F1}";
    }
}