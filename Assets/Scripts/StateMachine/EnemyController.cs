using TMPro;
using UnityEngine;

// Абстрактный контроллер врага.
// Регистрирует общие состояния и соединяет компоненты атаки и перемещения.
// Конкретный враг наследуется и при необходимости добавляет свои состояния через RegisterStates().
public abstract class EnemyController : AiController
{
    [SerializeField] protected EnemyAttackBase _attack;
    [SerializeField] protected EnemyMovementBase _movement;
    [SerializeField] DamageRangeFinder _detectionFinder;
    [SerializeField] protected BillboardAnimator _animator;
    [SerializeField] TextMeshPro _debugText;

    public EnemyAttackBase Attack => _attack;
    public EnemyMovementBase Movement => _movement;

    protected override void Awake()
    {
        _stateMachine.AddState(new EnemyIdleState(this, _stateMachine));
        _stateMachine.AddState(new EnemyMoveState(this, _stateMachine));
        _stateMachine.AddState(new EnemyAttackState(this, _stateMachine));
        RegisterStates();
        _stateMachine.Initialize<EnemyIdleState>();

        if (_unit is EnemyUnitMono enemyUnit)
            enemyUnit.OnDied += HandleDied;
    }

    protected virtual void RegisterStates() { }

    void HandleDied()
    {
        _attack?.CancelWindup();
        _unit.StopMovement();
        enabled = false; // останавливает Update → стейт-машина замирает
        OnDied();
    }

    // Переопределяй в подклассе для анимации смерти и Destroy.
    protected virtual void OnDied() => Destroy(gameObject);

    public void SetDebugInfo(string stateName, string param)
    {
        if (_debugText == null) return;
        _debugText.text = $"{stateName}\n{param}";
    }

    // Ближайший IHittable в радиусе обнаружения. null если никого нет.
    public Transform GetTarget()
    {
        Transform nearest = null;
        float minSqDist = float.MaxValue;
        foreach (var hittable in _detectionFinder.InRange)
        {
            if (hittable is not Component c || !c) continue;
            float sqDist = (c.transform.position - transform.position).sqrMagnitude;
            if (sqDist < minSqDist) { minSqDist = sqDist; nearest = c.transform; }
        }
        return nearest;
    }

    // Хуки смены состояний — переопределяются в подклассах для анимации.
    public virtual void OnEnterIdle() {}
    public virtual void OnEnterMove() {}
    public virtual void OnEnterAttack() {}
}
