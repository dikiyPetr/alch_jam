using UnityEngine;

public class MergeSkill : SlimeSkill
{
    public override void Update(SlimeMono unit, float dt)
    {
        if (SlimePoolMono.Instance.IsControlled(unit))
        {
            IsComplete = true;
            return;
        }

        if (unit.Data.CurrentState == Slime.State.Dead) return;

        var nearest = SlimePoolMono.Instance.FindNearestTo(unit.transform.position, unit);
        if (nearest == null)
        {
            IsComplete = true;
            return;
        }

        var midpoint = (unit.transform.position + nearest.transform.position) * 0.5f;
        unit.NavigateTo(midpoint, unit.Pool.MoveSpeed * unit.Pool.MergeSpeedMultiplier);

        if (Vector2.Distance(new Vector2(unit.transform.position.x, unit.transform.position.z),
                new Vector2(nearest.transform.position.x, nearest.transform.position.z)) <= unit.Pool.MergeRadius)
            if (unit.Data.CurrentHp >= nearest.Data.CurrentHp)
                // unit поглощает nearest: суммирует урон и HP
                unit.AbsorbMono(nearest);
            else
                // nearest поглощает unit: суммирует урон и HP
                nearest.AbsorbMono(unit);
    }

    public override void OnDeactivate(SlimeMono unit)
    {
        unit.SetMode(UnitMono.MoveMode.Controller);
    }
}