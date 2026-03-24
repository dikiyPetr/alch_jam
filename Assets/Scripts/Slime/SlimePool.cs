using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlimePool
{
    // Все значения живут в SlimeConfig — здесь только шорткаты для удобства
    public float HpPerUnit => SlimeConfig.Instance.hpPerUnit;
    public float MoveSpeed => SlimeConfig.Instance.moveSpeed;
    public float MergeSpeedMultiplier => SlimeConfig.Instance.mergeSpeedMultiplier;
    public float BaseUnitScale => SlimeConfig.Instance.baseUnitScale;
    public float SplitSpawnRadius => SlimeConfig.Instance.splitSpawnRadius;
    public float MergeRadius => SlimeConfig.Instance.mergeRadius;
    public float SkillDuration => SlimeConfig.Instance.skillDuration;
    public float ProjectileSpeedMultiplier => SlimeConfig.Instance.projectileSpeedMultiplier;
    public float ProjectileReturnDelay     => SlimeConfig.Instance.projectileReturnDelay;
    public float ProjectileSteeringSpeed   => SlimeConfig.Instance.projectileSteeringSpeed;

    private readonly List<Slime> _slimes = new();
    public IReadOnlyList<Slime> Slimes => _slimes;

    [SerializeField] private float _totalMaxHp;
    [SerializeField] private int _totalUnitCount;

    public float TotalMaxHp => _totalMaxHp;
    public int TotalUnitCount => _totalUnitCount;

    public Slime CreateSlime(float initialMaxHp) => new Slime(this, initialMaxHp);


    internal void Register(Slime slime)
    {
        if (!_slimes.Contains(slime))
            _slimes.Add(slime);
        RefreshDebugStats();
    }

    internal void Unregister(Slime slime)
    {
        _slimes.Remove(slime);
        RefreshDebugStats();
    }

    private void RefreshDebugStats()
    {
        float total = 0f;
        foreach (var s in _slimes) total += s.ContainedMaxHp;
        _totalMaxHp = total;
        _totalUnitCount = (int)(total / HpPerUnit);
    }
}