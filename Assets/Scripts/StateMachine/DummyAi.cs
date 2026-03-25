using UnityEngine;

public class DummyAi : AiController
{
  private HomeData _homeData = new HomeData();

  private new void Awake()
  {
    _stateMachine.AddState(new IdleState(this, _stateMachine));
    _stateMachine.AddState(new FollowState(this, _stateMachine));
    _stateMachine.AddState(new MoveHomeState(this, _stateMachine, _homeData));

    _stateMachine.Initialize<IdleState>();
  }

  public void SetGroupToHomeData(Group @group) => _homeData.@group = group;

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