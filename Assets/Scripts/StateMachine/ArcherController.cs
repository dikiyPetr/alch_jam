using UnityEngine;

// Контроллер лучника.
// Переключает анимации при смене состояния и в начале наводки.
public class ArcherController : EnemyController
{
    [SerializeField] SpriteAnimClip _clipIdle;
    [SerializeField] SpriteAnimClip _clipWalk;
    [SerializeField] SpriteAnimClip _clipAttack;

    protected override void Awake()
    {
        base.Awake();

        _attack.WindupStarted += () => _animator.Play(_clipAttack, forceRestart: true);

        _animator.OnEventFrame += (clip, _) =>
        {
            if (clip == _clipAttack) _attack.TriggerAttack();
        };

        _animator.OnClipEnd += clip =>
        {
            if (clip == _clipAttack) _animator.Play(_clipIdle);
        };
    }

    public override void OnEnterIdle() => _animator?.Play(_clipIdle);
    public override void OnEnterMove() => _animator?.Play(_clipWalk);
    // При входе в AttackState ждём наводки — остаёмся в idle пока не начнётся windup.
    public override void OnEnterAttack() => _animator?.Play(_clipIdle);
}
