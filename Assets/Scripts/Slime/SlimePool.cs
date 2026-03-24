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

    [field: SerializeField] public bool IsMerging { get; private set; }

    private readonly List<Slime> _slimes = new();
    public IReadOnlyList<Slime> Slimes => _slimes;

    [SerializeField] private float _totalMaxHp;
    [SerializeField] private int _totalUnitCount;

    public float TotalMaxHp => _totalMaxHp;
    public int TotalUnitCount => _totalUnitCount;

    public Slime CreateSlime(float initialMaxHp) => new Slime(this, initialMaxHp);

    // Запустить глобальный мерж всех слаймов
    public void StartMerge()
    {
        if (_slimes.Count <= 1) return;
        IsMerging = true;
        foreach (var s in new List<Slime>(_slimes))
            s.SetState(Slime.State.Merging);
    }

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
        if (IsMerging && _slimes.Count <= 1)
            StopMerge();
    }

    private void RefreshDebugStats()
    {
        float total = 0f;
        foreach (var s in _slimes) total += s.ContainedMaxHp;
        _totalMaxHp = total;
        _totalUnitCount = (int)(total / HpPerUnit);
    }

    private void StopMerge()
    {
        IsMerging = false;
        foreach (var s in _slimes)
            s.SetState(Slime.State.Autonomous);
    }
}
