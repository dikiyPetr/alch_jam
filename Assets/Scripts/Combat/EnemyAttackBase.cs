using System;
using UnityEngine;

// Абстрактный компонент атаки врага.
// Управляет кулдауном и фазой наводки (windup); конкретная реализация — в DoAttack().
// Вызывается из EnemyAttackState.
public abstract class EnemyAttackBase : MonoBehaviour
{
    [SerializeField] float _attackRange = 5f;
    [SerializeField] float _attackCooldown = 2f;
    [SerializeField] float _windupDuration = 0.5f;

    // Переопределяй в подклассе чтобы задать длительность наводки программно.
    protected virtual float WindupDuration => _windupDuration;

    float _nextAttackTime;
    bool _inWindup;
    float _windupEndTime;
    Transform _windupTarget;

    public float AttackRange => _attackRange;

    // Во время наводки показывает оставшееся время наводки, иначе — кулдаун.
    public float CooldownRemaining => _inWindup
        ? Mathf.Max(0f, _windupEndTime - Time.time)
        : Mathf.Max(0f, _nextAttackTime - Time.time);

    public bool IsWindingUp => _inWindup;

    // Срабатывает в момент начала фазы наводки (можно подписаться снаружи).
    public event Action WindupStarted;

    public void TryAttack(Transform target)
    {
        if (_inWindup)
        {
            _windupTarget = target;
            OnWindupTick(target);
            if (Time.time >= _windupEndTime) TriggerAttack(); // fallback если нет анимсобытия
            return;
        }

        if (Time.time < _nextAttackTime) return;

        _windupTarget = target;
        _inWindup = true;
        _windupEndTime = Time.time + WindupDuration;
        WindupStarted?.Invoke();
        OnWindupStart(target);
    }

    // Выстрелить немедленно — вызывается из анимационного события или по таймауту.
    public void TriggerAttack()
    {
        if (!_inWindup) return;
        _inWindup = false;
        DoAttack(_windupTarget);
        _nextAttackTime = Time.time + _attackCooldown;
    }

    public void CancelWindup()
    {
        if (!_inWindup) return;
        _inWindup = false;
        OnWindupCancelled();
    }

    // Вызывается в начале фазы наводки.
    protected virtual void OnWindupStart(Transform target) {}

    // Вызывается каждый кадр во время наводки (до DoAttack).
    protected virtual void OnWindupTick(Transform target) {}

    // Вызывается при прерывании наводки (смена стейта, потеря цели).
    protected virtual void OnWindupCancelled() {}

    protected abstract void DoAttack(Transform target);
}
