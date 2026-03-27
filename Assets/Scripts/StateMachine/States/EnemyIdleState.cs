// Ожидание: переходит в EnemyMoveState при появлении цели в радиусе DamageRangeFinder.

public class EnemyIdleState : State
{
    public override float ExitDelay => 0.3f;
    readonly EnemyController _enemy;

    public EnemyIdleState(EnemyController controller, StateMachine stateMachine)
        : base(controller, stateMachine) => _enemy = controller;

    public override void Enter()
    {
        _enemy.SetDebugInfo("Idle", "");
        _enemy.OnEnterIdle();
    }

    public override void Update()
    {
        if (_enemy.GetTarget() != null)
            ChangeState<EnemyMoveState>();
    }
}