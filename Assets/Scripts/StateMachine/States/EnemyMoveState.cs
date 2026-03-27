// Движение к позиции атаки: делегирует EnemyMovementBase.UpdateMovement().
// Переходит в EnemyAttackState, когда Movement.IsInPosition() == true.

using UnityEngine;

public class EnemyMoveState : State
{
    readonly EnemyController _enemy;

    public EnemyMoveState(EnemyController controller, StateMachine stateMachine)
        : base(controller, stateMachine) => _enemy = controller;

    public override void Enter() => _enemy.OnEnterMove();
    public override void Exit() => _enemy.Movement.ResetMovement(_enemy.Unit);

    public override void Update()
    {
        var target = _enemy.GetTarget();
        if (target == null) { ChangeState<EnemyIdleState>(); return; }

        float dist = Vector3.Distance(_enemy.Unit.transform.position, target.position);
        _enemy.SetDebugInfo("Move", $"dist: {dist:F1}");

        if (_enemy.Movement.IsInPosition(_enemy.Unit.transform, target))
        {
            ChangeState<EnemyAttackState>();
            return;
        }

        _enemy.Movement.UpdateMovement(_enemy.Unit, target);
    }
}
