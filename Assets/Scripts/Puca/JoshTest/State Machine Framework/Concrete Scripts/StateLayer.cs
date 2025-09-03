using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateLayer
{
    public IState CurrentState { get; private set; }
    public void SetState(IState newState)
    {
        if (CurrentState == newState) return;
        CurrentState?.ExitState();
        CurrentState = newState;
        CurrentState?.EnterState();
    }
    public void Update()
    {
        CurrentState?.Update();
    }
    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
}
