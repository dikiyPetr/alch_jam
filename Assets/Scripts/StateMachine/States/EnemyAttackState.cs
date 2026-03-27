// Атака: делегирует EnemyAttackBase.TryAttack() каждый кадр (кулдаун внутри компонента).
// Возвращается в EnemyMoveState, если цель вышла из позиции.
public class EnemyAttackState : State
{
    readonly EnemyController _enemy;

    public EnemyAttackState(EnemyController controller, StateMachine stateMachine)
        : base(controller, stateMachine) => _enemy = controller;

    public override void Enter()
    {
        _enemy.Unit.StopMovement();
        _enemy.OnEnterAttack();
    }
    public override void Exit() => _enemy.Attack.CancelWindup();

    public override void Update()
    {
        var target = _enemy.GetTarget();
        if (target == null) { ChangeState<EnemyIdleState>(); return; }

        _enemy.Attack.TryAttack(target);
        _enemy.SetDebugInfo("Attack", $"cd: {_enemy.Attack.CooldownRemaining:F1}");

        if (!_enemy.Movement.IsInPosition(_enemy.Unit.transform, target))
            ChangeState<EnemyMoveState>();
    }
}
