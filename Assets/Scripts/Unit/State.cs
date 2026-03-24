public abstract class State
{
  protected Unit unit;
  protected StateMachine sm;

  public State(Unit unit, StateMachine sm)
  {
    this.unit = unit;
    this.sm = sm;
  }

  protected void ChangeState<T>() where T : State
  {
    sm.ChangeState<T>();
  }

  public virtual void Enter() {}
  public virtual void Exit() {}
  public virtual void Update() {}
}