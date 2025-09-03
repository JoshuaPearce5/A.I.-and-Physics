using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class StatusSlowedState : PlayerStatusState
{
    public float slowedCoyoteTime = 0.5f;

    public float slowedCoyoteCounter;

    public StatusSlowedState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Player Entered Slowed State!");

        controller.data.maxRunSpeed = 5f;

        controller.data.maxFlySpeed = 3.5f;

        slowedCoyoteCounter = slowedCoyoteTime;

    }
    public override void ExitState()
    {
        controller.data.maxRunSpeed = 10f;

        controller.data.maxFlySpeed = 7f;
    }
    public override void Update()
    {
        if (slowedCoyoteCounter > 0.1f)
        {
            slowedCoyoteCounter -= Time.deltaTime;
        }
        else if (slowedCoyoteCounter <= 0.1f)
        {
            controller.statusLayer.SetState(new StatusDefaultState(controller));
        }
    }
    public override void FixedUpdate()
    {
        return;
    }
}
