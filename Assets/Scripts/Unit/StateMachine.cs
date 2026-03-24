using System;
using System.Collections.Generic;

public class StateMachine
{
  private Dictionary<Type, State> states = new Dictionary<Type, State>();
  private State currentState;

  public T GetState<T>() where T : State
  {
    states.TryGetValue(typeof(T), out var state);
    return (T)state;
  }

  public void AddState(State state)
  {
    states[state.GetType()] = state;
  }

  public void Initialize<T>() where T : State
  {
    ChangeState<T>();
  }

  public void ChangeState<T>() where T : State
  {
    Type type = typeof(T);

    if (!states.ContainsKey(type))
    {
      UnityEngine.Debug.LogWarning($"State {type} not found in FSM");
      return;
    }

    currentState?.Exit();
    currentState = states[type];
    currentState.Enter();
  }

  public void Update()
  {
    currentState?.Update();
  }
}