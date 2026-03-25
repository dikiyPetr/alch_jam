using UnityEngine;

public class MoveHomeState : State
{
  private Vector3 _homePosition;

  public MoveHomeState(AiController aiController, StateMachine stateMachine) : base(aiController, stateMachine)
  {
    _homePosition = aiController.transform.position;
  }

  public override void Update()
  {
    aiController.Unit.NavigateTo(_homePosition, 3);
    if (_homePosition == aiController.Unit.transform.position)
    {
      stateMachine.ChangeState<IdleState>();
    }
  }
}