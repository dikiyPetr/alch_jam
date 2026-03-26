using UnityEngine;

// Визуал падающего метеора — обновляет localPosition _meteor от _spawnHeight до 0 за Duration секунд.
public class MeteorFallVisual : TimedVisual
{
    [SerializeField] Transform _meteor;
    [SerializeField] float _spawnHeight = 10f;

    float _elapsed;

    public override void StartVisual()
    {
        _elapsed = 0f;
        if (_meteor != null) _meteor.localPosition = Vector3.up * _spawnHeight;
        base.StartVisual();
    }

    void Update()
    {
        if (_meteor == null) return;
        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / Duration);
        _meteor.localPosition = Vector3.up * Mathf.Lerp(_spawnHeight, 0f, t);
    }
}
