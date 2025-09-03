using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDefaultState : PlayerStatusState
{
    public StatusDefaultState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Player Entered Default State");
    }
    public override void ExitState()
    {
        return;    
    }
    public override void Update()
    {
        return;
    }
    public override void FixedUpdate()
    {
        return;
    }
}
