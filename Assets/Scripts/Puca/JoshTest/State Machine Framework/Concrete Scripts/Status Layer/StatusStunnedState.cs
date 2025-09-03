using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusStunnedState : PlayerStatusState
{
    public float stunnedCoyoteTime = 0.3f;
    public float stunnedCoyoteCounter;

    public StatusStunnedState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Player Entered Stunned State!");

        Vector2 velocity = controller.data.currentVelocity;

        velocity.x = 0f;
        velocity.y = 0f;

        controller.data.controlsDisabled = true;

        controller.data.isStunned = true;

        controller.data.currentVelocity = velocity;

        stunnedCoyoteCounter = stunnedCoyoteTime;
    }
    public override void ExitState()
    {
        controller.data.controlsDisabled = false;
        
        controller.data.isStunned = false;
    }
    public override void Update()
    {
        if (stunnedCoyoteCounter > 0.1f)
        {
            stunnedCoyoteCounter -= Time.deltaTime;
        }
        else if (stunnedCoyoteCounter <= 0.1f)
        {
            controller.statusLayer.SetState(new StatusSlowedState(controller));
        }
    }
    public override void FixedUpdate()
    {
        return;
    }
}
