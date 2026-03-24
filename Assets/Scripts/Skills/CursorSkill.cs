using UnityEngine;

/// <summary>
/// Слайм движется к точке курсора (захватывается один раз при активации),
/// затем ищет наибольший слайм и возвращается к нему для слияния.
/// </summary>
public class CursorSkill : SlimeSkill
{
    Vector3 _cursorTarget;
    bool _returning;
    private float skillDuration;

    public CursorSkill(float poolSkillDuration)
    {
        skillDuration = poolSkillDuration;
    }

    public override void OnActivate(SlimeMono unit)
    {
        _cursorTarget = CursorWorldPosition.Instance.Position;
        // TODO: зарефакторить вызов перемещения
        // unit.ApplyMovement(_cursorTarget, GetMoveSpeed(unit));
    }

    public override void Update(SlimeMono unit, float dt)
    {
        var mono = (SlimeMono)unit;

        if (!_returning)
        {
            if (HorizontalDistance(unit.transform.position, _cursorTarget) <= mono.Pool.MergeRadius)
                _returning = true;
        }

        if (_returning)
        {
            if (mono.Pool.IsMerging)
            {
                IsComplete = true;
                return;
            }

            ReturnToLargest(mono);
        }
    }

    void ReturnToLargest(SlimeMono mono)
    {
        var target = SlimePoolMono.Instance.FindLargestExcept(mono);
        if (target == null)
        {
            IsComplete = true;
            return;
        }

        // TODO: зарефакторить вызов перемещения
        // mono.ApplyMovement(target.transform.position, mono.Pool.MoveSpeed);

        if (HorizontalDistance(mono.transform.position, target.transform.position) <= mono.Pool.MergeRadius)
        {
            target.Data.Absorb(mono.Data);
            IsComplete = true;
        }
    }

    static float GetMoveSpeed(SlimeMono unit)
        => unit is SlimeMono slime ? slime.Pool.MoveSpeed : SlimeConfig.Instance.moveSpeed;

    static float HorizontalDistance(Vector3 a, Vector3 b)
        => Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
}