using UnityEngine;

/// <summary>
/// Фаза 1 («полёт»): одноразовый импульс в зафиксированном направлении.
///   Velocity не переопределяется каждый кадр — физика Unity сама обрабатывает
///   столкновения, отскоки (через PhysicMaterial на коллайдере) и толчки объектов.
///
/// Фаза 2 («возврат»): по истечении таймера активирует MergeSkill на этом слайме.
/// </summary>
public class ProjectileSkill : SlimeSkill
{
    readonly Vector3 _direction; // направление броска, зафиксированное при активации
    readonly float _returnDelay; // время до начала возврата (секунды)
    readonly float _speedMultiplier;

    float _timer;

    public ProjectileSkill(Vector3 direction, float returnDelay, float speedMultiplier)
    {
        _direction = direction;
        _returnDelay = returnDelay;
        _speedMultiplier = speedMultiplier;
    }

    public override void OnActivate(SlimeMono unit)
    {
        unit.SetLinearDrag(0f);
        // Единственный вызов задания скорости — дальше физика сама
        unit.ApplyImpulse(_direction * unit.Pool.MoveSpeed * _speedMultiplier);
    }

    public override void Update(SlimeMono unit, float dt)
    {
        _timer += dt;

        if (_timer >= _returnDelay)
        {
            IsComplete = true;
        }
    }
}