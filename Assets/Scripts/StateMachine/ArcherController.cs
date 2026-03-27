using UnityEngine;

// Контроллер лучника.
// Переключает анимации при смене состояния, в начале наводки, на хит и смерть.
public class ArcherController : EnemyController
{
    [SerializeField] SpriteAnimClip _clipIdle;
    [SerializeField] SpriteAnimClip _clipWalk;
    [SerializeField] SpriteAnimClip _clipAttack;
    [SerializeField] SpriteAnimClip _clipDeath;
    [SerializeField] Color _hitFlashColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] float _hitFlashDuration = 0.15f;

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
            else if (clip == _clipDeath) Destroy(gameObject);
        };

        if (_unit is EnemyUnitMono eu)
            eu.OnHit += _ => _animator.Flash(_hitFlashColor, _hitFlashDuration);
    }

    protected override void OnDied()
    {
        _animator.Play(_clipDeath, forceRestart: true);
        // Destroy вызовется из OnClipEnd когда анимация смерти закончится.
    }

    public override void OnEnterIdle() => _animator?.Play(_clipIdle);
    public override void OnEnterMove() => _animator?.Play(_clipWalk);
    public override void OnEnterAttack() => _animator?.Play(_clipIdle);
}
