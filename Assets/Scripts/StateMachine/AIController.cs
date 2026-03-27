using UnityEngine;

public abstract class AiController : MonoBehaviour
{
  
  protected StateMachine _stateMachine = new StateMachine();

  [SerializeField] protected UnitMono _unit;

  public UnitMono Unit => _unit;

  protected virtual void Awake()
  {
    _stateMachine.AddState(new IdleState(this, _stateMachine));

    _stateMachine.Initialize<IdleState>();
  }

  void Update()
  {
    _stateMachine.Update();
  }

  public void SetState<T>() where T : State
  {
    _stateMachine.ChangeState<T>();
  }
}