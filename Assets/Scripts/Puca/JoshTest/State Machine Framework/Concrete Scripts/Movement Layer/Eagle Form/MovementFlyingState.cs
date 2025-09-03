using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MovementFlyingState : PlayerMovementState
{
    public MovementFlyingState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Entering Flying State");

        Vector2 velocity = controller.data.currentVelocity;

        velocity.y = controller.data.upwardFlySpeed;

        controller.data.currentVelocity.y = velocity.y;
    }
    public override void ExitState()
    {
    }
    public override void Update()
    {
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
            else if (Input.GetButtonDown("Jump"))
            {
                controller.movementLayer.SetState(new MovementFlyingUpState(controller));
            }
        }
    }
    public override void FixedUpdate()
    {
        ApplyMovement();
        ApplyGravity();
    }
    public void ApplyMovement()
    {
        if (controller.data.moveDirection != 0)
        {
            Vector2 velocity = controller.data.currentVelocity;

            float targetSpeed = controller.data.moveDirection * controller.data.maxFlySpeed;
            float speedDiff = targetSpeed - velocity.x;
            float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? controller.data.runAcceleration : controller.data.runDeceleration;

            // Apply acceleration
            velocity.x += speedDiff * accelerationRate;

            // Clamp velocity.x to maxFlySpeed
            velocity.x = Mathf.Clamp(velocity.x, -controller.data.maxFlySpeed, controller.data.maxFlySpeed);
            controller.data.currentVelocity.x = velocity.x;
        }
        
    }
    public void ApplyGravity()
    {
        Vector2 velocity = controller.data.currentVelocity;

        if (!controller.data.isGrounded)
        {

            velocity.y += controller.data.gravityScale/4;
        }
        else if (velocity.y < 0)
        {
            velocity.y = 0;
        }

        controller.data.currentVelocity.y = velocity.y;

    }
}
