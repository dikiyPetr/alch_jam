using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Манекен для получения урона — показывает Total, Last hit и скользящий DPS.
public class HitReceiverDummy : MonoBehaviour, IHittable
{
    [Layer, SerializeField] int _layer;
    [SerializeField] TextMeshPro _totalText;
    [SerializeField] float _dpsWindow = 3f;

    float _totalDamage;
    float _lastHit;

    readonly Queue<(float time, float amount)> _hits = new();

    void Awake() => gameObject.layer = _layer;

    public void TakeDamage(float amount)
    {
        _totalDamage += amount;
        _lastHit = amount;
        _hits.Enqueue((Time.time, amount));
    }

    void Update()
    {
        float cutoff = Time.time - _dpsWindow;
        while (_hits.Count > 0 && _hits.Peek().time < cutoff)
            _hits.Dequeue();

        float windowDamage = 0f;
        foreach (var h in _hits) windowDamage += h.amount;
        float dps = windowDamage / _dpsWindow;

        _totalText.text = $"Total: {_totalDamage:F0}\nLast:  {_lastHit:F0}\nDPS:   {dps:F1}";
    }
}