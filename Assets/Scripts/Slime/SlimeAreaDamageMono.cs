using UnityEngine;

public class SlimeAreaDamageMono : AreaDamageMono
{
    [SerializeField] private SlimeMono _slime;

    // Вызывается при спавне или после изменения stats (сплит, слияние)
    public void Initialize(SlimeMono slime)
    {
        _slime = slime;
        SetTickRate(slime.Stats.radiusTickRate);
        // Устанавливаем радиус коллайдера из stats слайма
        if (_rangeFinder != null && _rangeFinder.Collider is SphereCollider sphere)
            sphere.radius = slime.Stats.radiusRange;
    }

    protected override float GetDamage()
    {
        return _slime != null ? _slime.Stats.radiusDamage.amount * _slime.DamageMultiplier : 0f;
    }
}