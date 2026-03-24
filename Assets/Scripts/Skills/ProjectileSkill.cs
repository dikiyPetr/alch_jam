using UnityEngine;

/// <summary>
/// Фаза 1 («полёт»): одноразовый импульс в зафиксированном направлении.
///   Velocity не переопределяется каждый кадр — физика Unity сама обрабатывает
///   столкновения, отскоки (через PhysicMaterial на коллайдере) и толчки объектов.
///
/// Фаза 2 («возврат»): плавное наведение на наибольший слайм через SteerTowards,
///   что даёт инерционный разворот вместо мгновенного.
/// </summary>
public class ProjectileSkill : SlimeSkill
{
    readonly Vector3 _direction;      // направление броска, зафиксированное при активации
    readonly float _returnDelay;      // время до начала возврата (секунды)
    readonly float _speedMultiplier;

    float _originalDrag;
    float _timer;
    bool _returning;

    public ProjectileSkill(Vector3 direction, float returnDelay, float speedMultiplier)
    {
        _direction = direction;
        _returnDelay = returnDelay;
        _speedMultiplier = speedMultiplier;
    }

    public override void OnActivate(SlimeMono unit)
    {
        unit.MoveTarget = unit.transform.position + _direction * 5f;

        // Убираем drag на время полёта, чтобы импульс не гас сразу
        _originalDrag = unit.GetLinearDrag();
        unit.SetLinearDrag(0f);

        // Единственный вызов задания скорости — дальше физика сама
        unit.ApplyImpulse(_direction * unit.Pool.MoveSpeed * _speedMultiplier);
    }

    public override void Update(SlimeMono unit, float dt)
    {
        _timer += dt;

        if (!_returning)
        {
            // Фаза 1: не трогаем velocity — PhysicMaterial на коллайдере обеспечит отскоки
            if (_timer >= _returnDelay)
            {
                _returning = true;
                unit.MoveTarget = unit.transform.position;
            }
            return;
        }

        // Фаза 2: ищем цель и плавно наводимся на неё
        var target = SlimePoolMono.Instance.FindLargestExcept(unit);
        if (target == null)
        {
            IsComplete = true;
            return;
        }

        unit.MoveTarget = target.transform.position;

        var toTarget = target.transform.position - unit.transform.position;
        toTarget.y = 0f;

        // SteerTowards даёт инерционный разворот, а не мгновенное переключение направления
        unit.SteerTowards(
            toTarget.normalized,
            unit.Pool.MoveSpeed * _speedMultiplier,
            unit.Pool.ProjectileSteeringSpeed,
            dt);

        float dist = Vector2.Distance(
            new Vector2(unit.transform.position.x, unit.transform.position.z),
            new Vector2(target.transform.position.x, target.transform.position.z));

        if (dist <= unit.Pool.MergeRadius)
        {
            target.Data.Absorb(unit.Data);
            IsComplete = true;
        }
    }

    public override void OnDeactivate(SlimeMono unit)
    {
        // Восстанавливаем drag, который был у слайма до броска
        unit.SetLinearDrag(_originalDrag);
    }
}
