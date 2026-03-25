using UnityEngine;

// Базовый класс для компонентов нанесения урона через коллизии.
// Все зависимости (юнит, маска слоёв) задаются через поля инспектора.
public abstract class DamageSourceMono : MonoBehaviour
{
    [SerializeField] protected LayerMask _targetLayers;
    [SerializeField] float _tickRate;   // 0 — без периодичности

    float _timer;

    // Позволяет подклассам (например SlimeAreaDamageMono) устанавливать тикрейт в рантайме
    protected void SetTickRate(float rate)
    {
        _tickRate = rate;
        _timer = 0f;
    }

    void Update()
    {
        if (_tickRate <= 0f) return;
        _timer += Time.deltaTime;
        if (_timer < _tickRate) return;
        _timer = 0f;
        Tick();
    }

    // Вызывается каждый тик — переопредели в подклассе если нужна периодичность
    protected virtual void Tick() { }

    protected abstract float GetDamage();

    protected bool TryGetTarget(GameObject go, out IHittable target)
    {
        target = null;
        return IsInLayerMask(go.layer, _targetLayers) && go.TryGetComponent(out target);
    }

    static bool IsInLayerMask(int layer, LayerMask mask) => (mask.value & (1 << layer)) != 0;
}
