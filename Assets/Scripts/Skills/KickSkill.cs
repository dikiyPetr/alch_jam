// Даёт слайму свободно лететь по инерции заданное время.
// Пока скилл активен, Update() SlimeMono не вызывает StopMovement(),
// и Rigidbody тормозит только за счёт своего LinearDamping (drag).
public class KickSkill : SlimeSkill
{
    readonly float _duration;
    float _timer;

    public KickSkill(float duration)
    {
        _duration = duration;
    }

    public override void Update(SlimeMono unit, float dt)
    {
        _timer += dt;
        if (_timer >= _duration)
            IsComplete = true;
    }
}
