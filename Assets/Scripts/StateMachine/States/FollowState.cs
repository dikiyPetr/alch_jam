using System.Linq;
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
            aiController.Unit.NavigateTo(SlimePoolMono.Instance.GetControlledSlimes().First().transform.position, 3);
        }
    }
}