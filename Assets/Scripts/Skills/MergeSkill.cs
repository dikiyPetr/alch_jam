using UnityEngine;

public class MergeSkill : SlimeSkill
{
    public override void Update(SlimeMono unit, float dt)
    {
        if (SlimePoolMono.Instance.GetControlledSlime() == unit)
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
        var dir = (midpoint - unit.transform.position).normalized;
        dir.y = 0f;
        unit.MoveInDirection(dir, unit.Pool.MoveSpeed * unit.Pool.MergeSpeedMultiplier);

        if (Vector2.Distance(new Vector2(unit.transform.position.x, unit.transform.position.z),
                             new Vector2(nearest.transform.position.x, nearest.transform.position.z)) <= unit.Pool.MergeRadius)
            unit.Data.Absorb(nearest.Data);
    }
}