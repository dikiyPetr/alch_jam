using UnityEngine;

// Поведение перемещения: держит дистанцию между _minRange и _maxRange.
// IsInPosition: dist ∈ [_minRange, _maxRange].
// Слишком близко → отступить; слишком далеко → подойти.
public class KeepDistanceMovement : EnemyMovementBase
{
    [SerializeField] float _minRange = 5f;
    [SerializeField] float _maxRange = 8f;
    [SerializeField] float _moveSpeed = 3f;

    public override bool IsInPosition(Transform self, Transform target)
    {
        float dist = Vector3.Distance(self.position, target.position);
        return dist >= _minRange && dist <= _maxRange;
    }

    public override void UpdateMovement(UnitMono unit, Transform target)
    {
        float dist = Vector3.Distance(unit.transform.position, target.position);

        if (dist < _minRange)
        {
            Vector3 awayDir = (unit.transform.position - target.position).normalized;
            unit.NavigateTo(unit.transform.position + awayDir * _maxRange, _moveSpeed);
        }
        else
        {
            unit.NavigateTo(target.position, _moveSpeed);
        }
    }
}
