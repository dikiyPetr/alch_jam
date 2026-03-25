using System.Linq;
using UnityEngine;

public class FollowState : State
{
  public FollowState(AiController aiController, StateMachine stateMachine) : base(aiController, stateMachine)
  {
  }


  public override void Update()
  {
    aiController.Unit.NavigateTo(SlimePoolMono.Instance.GetControlledSlimes().First().transform.position, 3);
  }
}