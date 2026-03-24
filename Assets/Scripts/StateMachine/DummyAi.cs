using UnityEngine;

public class DummyAi : AiController
{
  private new void Awake()
  {
    _stateMachine.AddState(new IdleState(this, _stateMachine));
    _stateMachine.AddState(new FollowState(this, _stateMachine));
    _stateMachine.AddState(new MoveHomeState(this, _stateMachine));

    _stateMachine.Initialize<IdleState>();
  }

  [ContextMenu("Следуй")]
  void ActivateFollowState()
  {
    _stateMachine.ChangeState<FollowState>();
  }

  [ContextMenu("Домой")]
  void ActivateMoveHomeState()
  {
    _stateMachine.ChangeState<MoveHomeState>();
  }
}