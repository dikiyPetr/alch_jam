using System;
using System.Collections;
using UnityEngine;

// Снаряд мага: предупреждение + метеор одновременно → зона урона с огнём.
// При _instantSpawn=true — сразу активирует зону без фаз.
// Длительность каждой фазы задаётся здесь и прокидывается в визуалы.
public class MeteorAreaDamage : AreaDamageMono
{
    [SerializeField] float _damage = 20f;

    [Header("Длительности")] [SerializeField]
    float _warningDuration = 1.5f;

    [SerializeField] float _fallDuration = 1.5f;
    [SerializeField] float _impactDuration = 2f;

    [Header("Визуал")] [SerializeField] TimedVisual _warningVisual;
    [SerializeField] MeteorFallVisual _meteorVisual;
    [SerializeField] TimedVisual _fireVisual;

    public event Action<float> OnDamageDealt;

    // skipWarning=true — пропустить фазу предупреждения (если наводка уже показана снаружи).
    public void Launch(Vector3 targetPos, bool skipWarning = false)
    {
        transform.position = targetPos;
        SetDamageZoneActive(false);
        StartCoroutine(skipWarning ? SkipWarningSequence() : FullSequence());
    }

    IEnumerator FullSequence()
    {
        _warningVisual?.StartVisual(_warningDuration);
        _meteorVisual?.StartVisual(_fallDuration);
        yield return new WaitForSeconds(Mathf.Max(_warningDuration, _fallDuration));
        yield return ImpactPhase();
    }

    IEnumerator SkipWarningSequence()
    {
        _meteorVisual?.StartVisual(_fallDuration);
        yield return new WaitForSeconds(_fallDuration);
        yield return ImpactPhase();
    }

    IEnumerator ImpactPhase()
    {
        SetDamageZoneActive(true);
        _fireVisual?.StartVisual(_impactDuration);
        yield return new WaitForSeconds(_impactDuration);
        Destroy(gameObject);
    }

    void SetDamageZoneActive(bool active)
    {
        if (_rangeFinder != null)
            _rangeFinder.Collider.enabled = active;
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