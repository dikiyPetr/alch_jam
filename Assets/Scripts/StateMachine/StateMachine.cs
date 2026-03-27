using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    readonly Dictionary<Type, State> states = new();
    State _currentState;

    // Ожидающий переход: тип и оставшееся время задержки.
    Type _pendingState;
    float _exitTimer;

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
        ExecuteChange(typeof(T));
    }

    public void ChangeState<T>() where T : State
    {
        float delay = _currentState?.ExitDelay ?? 0f;
        if (delay > 0f)
        {
            if (_pendingState == null) _exitTimer = delay; // таймер стартует один раз
            _pendingState = typeof(T);                     // цель можно переопределить
            return;
        }
        ExecuteChange(typeof(T));
    }

    public void Update()
    {
        if (_pendingState != null)
        {
            _exitTimer -= Time.deltaTime;
            if (_exitTimer <= 0f)
            {
                var type = _pendingState;
                _pendingState = null;
                ExecuteChange(type);
            }
        }
        _currentState?.Update();
    }

    void ExecuteChange(Type type)
    {
        if (!states.ContainsKey(type))
        {
            Debug.LogWarning($"State {type} not found in FSM");
            return;
        }
        _currentState?.Exit();
        _currentState = states[type];
        _currentState.Enter();
    }
}
