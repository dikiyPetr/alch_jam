using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slime
{
    public enum State { Autonomous, Merging, Dead }

    public SlimePool Pool { get; }
    [field: SerializeField] public float ContainedMaxHp { get; private set; }
    [field: SerializeField] public float CurrentHp { get; private set; }
    [field: SerializeField] public State CurrentState { get; private set; }

    public int ContainedUnitCount => (int)(ContainedMaxHp / Pool.HpPerUnit);
    public bool CanSplit => ContainedUnitCount > 1;

    public event Action<Slime> OnStateChanged;
    public event Action<Slime> OnDied;

    internal Slime(SlimePool pool, float initialMaxHp)
    {
        Pool = pool;
        ContainedMaxHp = initialMaxHp;
        CurrentHp = initialMaxHp;
        CurrentState = State.Autonomous;
        pool.Register(this);
    }

    // --- Деление ---

    public Slime[] Split(int count = 1)
    {
        if (ContainedUnitCount <= count)
            throw new InvalidOperationException(
                $"Need >{count} units to split {count} off. Have {ContainedUnitCount}.");

        var result = new Slime[count];
        for (int i = 0; i < count; i++)
        {
            ContainedMaxHp -= Pool.HpPerUnit;
            CurrentHp = Math.Min(CurrentHp, ContainedMaxHp);
            result[i] = new Slime(Pool, Pool.HpPerUnit);
        }
        return result;
    }

    // --- Слияние ---

    // Этот слайм поглощает другой. Вызывается только со стороны поглощающего.
    public void Absorb(Slime other)
    {
        if (CurrentState == State.Dead || other.CurrentState == State.Dead) return;
        ContainedMaxHp += other.ContainedMaxHp;
        CurrentHp = Math.Min(CurrentHp + other.ContainedMaxHp, ContainedMaxHp);
        other.BeConsumed();
    }

    // Вызывается только из Absorb — не является "смертью"
    private void BeConsumed()
    {
        CurrentState = State.Dead;
        Pool.Unregister(this);
        OnStateChanged?.Invoke(this);
        // OnDied не вызывается — это слияние, а не гибель
    }

    // --- Состояние ---

    public void SetState(State state)
    {
        if (CurrentState == State.Dead) return;
        CurrentState = state;
        OnStateChanged?.Invoke(this);
    }

    // --- Урон и смерть ---

    public void TakeDamage(float amount)
    {
        if (CurrentState == State.Dead) return;
        CurrentHp = Math.Max(0f, CurrentHp - amount);
        if (CurrentHp <= 0f)
            Kill();
    }

    public void Kill()
    {
        if (CurrentState == State.Dead) return;
        CurrentState = State.Dead;
        Pool.Unregister(this);
        OnDied?.Invoke(this);
        OnStateChanged?.Invoke(this);
    }
}
