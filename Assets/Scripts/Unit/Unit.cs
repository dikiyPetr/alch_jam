using UnityEngine;

public class Unit : MonoBehaviour
{
  private Blackboard _blackboard = new Blackboard();
  private StateMachine _stateMachine;
  
  void Awake()
  {
    _stateMachine = new StateMachine();

    _stateMachine.AddState(new IdleState(this,_stateMachine));

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

  public void MoveTo(Vector3 pos)
  {
    transform.position = Vector3.MoveTowards(transform.position, pos, 5f * Time.deltaTime);
  }

  public void Attack(Unit target)
  {
    Debug.Log($"{name} атакует {target.name}");
  }

  public float DistanceTo(Unit other)
  {
    return Vector3.Distance(transform.position, other.transform.position);
  }
}