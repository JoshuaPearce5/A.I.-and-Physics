using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MovementAirborneState : PlayerMovementState
{
    public MovementAirborneState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Entering Airborne State");
        controller.animator.SetBool("isGrounded", false);
        controller.animator.SetBool("isMoving", false);
        controller.animator.SetBool("isFalling", true);
        controller.animator.SetBool("isJumping", false);
        controller.animator.SetBool("isDoubleJumping", false);
        controller.animator.SetBool("isWallHanging", false);
    }
    public override void ExitState()
    {
    }
    public override void Update()
    {
        if ((Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f || controller.data.isFacingWall) && controller.data.isGrounded)
        {
            controller.movementLayer.SetState(new MovementIdleState(controller));
        }

        if (!controller.data.controlsDisabled)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f && controller.data.isGrounded)
            {
                controller.movementLayer.SetState(new MovementRunState(controller));
            }
            else if (controller.data.isFacingWall && !controller.data.isGrounded)
            {
                controller.movementLayer.SetState(new MovementWallHangState(controller));
            }
            else if (Input.GetButtonDown("Jump") && !controller.data.isDoubleJumping)
            {
                controller.movementLayer.SetState(new MovementDoubleJumpState(controller));
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

            float targetSpeed = controller.data.moveDirection * controller.data.maxRunSpeed;
            float speedDiff = targetSpeed - velocity.x;
            float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? controller.data.runAcceleration : controller.data.runDeceleration;

            // Apply acceleration
            velocity.x += speedDiff * accelerationRate;

            // Clamp velocity.x to maxRunSpeed
            velocity.x = Mathf.Clamp(velocity.x, -controller.data.maxRunSpeed, controller.data.maxRunSpeed);
            controller.data.currentVelocity.x = velocity.x;
        }

    }
    public void ApplyGravity()
    {
        Vector2 velocity = controller.data.currentVelocity;

        if (!controller.data.isGrounded)
        {

            velocity.y += controller.data.gravityScale;
        }
        else if (velocity.y < 0)
        {
            velocity.y = 0;
        }

        controller.data.currentVelocity.y = velocity.y;
    }
}
