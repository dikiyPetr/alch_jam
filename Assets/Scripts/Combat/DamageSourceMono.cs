using UnityEngine;

// Базовый класс для компонентов нанесения урона.
// _rangeFinder — отдельный компонент с коллайдером и отслеживанием целей.
public abstract class DamageSourceMono : MonoBehaviour
{
    [SerializeField] protected DamageRangeFinder _rangeFinder;
    [SerializeField] LayerMask _targetLayers;
    [SerializeField] float _tickRate; // 0 — без периодичности

    float _timer;

    protected void Awake()
    {
        if (_rangeFinder != null)
            _rangeFinder.targetLayers = _targetLayers;
    }

    protected virtual void OnAwake()
    {
    }

    protected void SetTickRate(float rate)
    {
        _tickRate = rate;
        _timer = 0f;
    }

    protected void Update()
    {
        OnUpdate();
        if (_tickRate <= 0f) return;
        _timer += Time.deltaTime;
        if (_timer < _tickRate) return;
        _timer = 0f;
        Tick();
    }

    protected virtual void OnUpdate()
    {
    }

    protected virtual void Tick()
    {
    }

    protected abstract float GetDamage();
}