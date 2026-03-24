using UnityEngine;

public class MergeSkill : SlimeSkill
{
    public override void Update(SlimeMono unit, float dt)
    {
        if (unit.Data.CurrentState == Slime.State.Dead) return;

        var nearest = SlimePoolMono.Instance.FindNearestTo(unit.transform.position, unit);
        if (nearest == null) { IsComplete = true; return; }

        // TODO: зарефакторить вызов перемещения
        var midpoint = (unit.transform.position + nearest.transform.position) * 0.5f;
        // unit.NavigateTo(midpoint, unit.Pool.MoveSpeed * unit.Pool.MergeSpeedMultiplier);

        if (Vector3.Distance(unit.transform.position, nearest.transform.position) <= unit.Pool.MergeRadius)
            unit.Data.Absorb(nearest.Data);
    }
}
