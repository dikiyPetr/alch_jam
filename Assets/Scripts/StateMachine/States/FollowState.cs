using UnityEngine;

public class FollowState : State
{
  public FollowState(AiController aiController, StateMachine stateMachine) : base(aiController, stateMachine)
  {
  }

  private bool _isApplyNaviage = false;

  public override void Update()
  {
    if (!_isApplyNaviage)
    {
      aiController.Unit.NavigateTo(SlimePoolMono.Instance.GetControlledSlime().transform.position, 3);
    }
  }
}