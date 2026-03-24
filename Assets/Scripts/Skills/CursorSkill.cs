using UnityEngine;

/// <summary>
/// Слайм движется в сторону курсора <travelDuration> секунд,
/// затем ищет наибольший слайм в пуле и возвращается к нему для слияния.
/// </summary>
public class CursorSkill : SlimeSkill
{
    readonly float _travelDuration;
    float _elapsed;
    bool _returning;

    public CursorSkill(float travelDuration)
    {
        _travelDuration = travelDuration;
    }

    public override void Update(SlimeMono mono, float dt)
    {
        _elapsed += dt;

        if (!_returning)
        {
            if (_elapsed >= _travelDuration)
                _returning = true;
            else
                MoveTowardCursor(mono);
        }

        if (_returning)
        {
            if (mono.Pool.IsMerging) { IsComplete = true; return; }
            ReturnToLargest(mono);
        }
    }

    void MoveTowardCursor(SlimeMono mono)
    {
        var target = CursorWorldPosition.Instance.Position;
        var dir = target - mono.transform.position;
        dir.y = 0f;
        mono.MoveTarget = new Vector3(target.x, mono.transform.position.y, target.z);
        mono.ApplyMovement(dir.normalized);
    }

    void ReturnToLargest(SlimeMono mono)
    {
        var target = SlimePoolMono.Instance.FindLargestExcept(mono);
        if (target == null) { IsComplete = true; return; }

        mono.MoveTarget = new Vector3(target.transform.position.x, mono.transform.position.y, target.transform.position.z);
        var dir = target.transform.position - mono.transform.position;
        dir.y = 0f;
        mono.ApplyMovement(dir.normalized);

        if (dir.magnitude <= mono.Pool.MergeRadius)
        {
            target.Data.Absorb(mono.Data);
            IsComplete = true;
        }
    }

}
