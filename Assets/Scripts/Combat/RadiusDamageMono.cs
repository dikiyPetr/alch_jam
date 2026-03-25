using UnityEngine;

// Периодический урон в радиусе вокруг слайма.
// Периодичность задаётся через _tickRate в DamageSourceMono.
[RequireComponent(typeof(SphereCollider))]
public class RadiusDamageMono : AreaDamageMono
{
    [SerializeField] SlimeMono _slime;

    protected override float GetDamage()
        => _slime.Stats.radiusDamage.amount * _slime.Data.ContainedUnitCount;
}
