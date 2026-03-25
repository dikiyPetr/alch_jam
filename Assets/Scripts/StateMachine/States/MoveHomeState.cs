using UnityEngine;

public class HomeData
{
  public Group group;
}
public class MoveHomeState : State
{
  private HomeData _data;
  public MoveHomeState(AiController aiController, StateMachine stateMachine, HomeData data) : base(aiController, stateMachine)
  {
    _data = data;
  }

  public override void Update()
  {
    aiController.Unit.NavigateTo(_data.@group.transform.position, 3);
  }
}