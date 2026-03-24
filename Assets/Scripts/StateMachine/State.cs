public abstract class State
{
  protected readonly AiController aiController;
  protected readonly StateMachine stateMachine;

  protected State(AiController aiController, StateMachine stateMachine)
  {
    this.aiController = aiController;
    this.stateMachine = stateMachine;
  }

  protected void ChangeState<T>() where T : State
  {
    stateMachine.ChangeState<T>();
  }

  public virtual void Enter() {}
  public virtual void Exit() {}
  public virtual void Update() {}
}