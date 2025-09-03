using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MovementFlyingUpState : PlayerMovementState
{
    public float jumpCoyoteTime = 0.1f;
    public float jumpCoyoteCounter = 0f;

    public MovementFlyingUpState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Entering Flying Up State");

        jumpCoyoteCounter = jumpCoyoteTime;

        Vector2 velocity = controller.data.currentVelocity;

        velocity.y = controller.data.upwardFlySpeed;

        controller.data.currentVelocity.y = velocity.y;

        controller.energy.BurstEnergy(10f);
    }
    public override void ExitState()
    {
    }
    public override void Update()
    {
        if (jumpCoyoteCounter > 0f)
        {
            jumpCoyoteCounter -= Time.deltaTime;
        }

        if ((Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f || controller.data.isFacingWall) && controller.data.isGrounded)
        {
            controller.movementLayer.SetState(new MovementIdleState(controller));
            controller.formLayer.SetState(new FormDefaultState(controller));
        }

        if (!controller.data.controlsDisabled)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f && controller.data.isGrounded)
            {
                controller.movementLayer.SetState(new MovementRunState(controller));
                controller.formLayer.SetState(new FormDefaultState(controller));
            }
            else if (controller.data.isFacingWall && !controller.data.isGrounded)
            {
                controller.movementLayer.SetState(new MovementWallHangState(controller));
                controller.formLayer.SetState(new FormDefaultState(controller));
            }
            else if (Input.GetButtonUp("Fly") || controller.energy.currentEnergy <= 0)
            {
                controller.movementLayer.SetState(new MovementAirborneState(controller));
            }
            else if(jumpCoyoteCounter <= 0.1f)
            {
                controller.movementLayer.SetState(new MovementFlyingState(controller));
            }
        }
    }
    public override void FixedUpdate()
    {
        return;
    }
}
