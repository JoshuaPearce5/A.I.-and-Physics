using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MovementRunState : PlayerMovementState
{
    public MovementRunState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Entering Run State");
        controller.animator.SetBool("isGrounded", true);
        controller.animator.SetBool("isMoving", true);
        controller.animator.SetBool("isFalling", false);
        controller.animator.SetBool("isJumping", false);
        controller.animator.SetBool("isDoubleJumping", false);
        controller.animator.SetBool("isWallHanging", false);

        // Reset vertical velocity when grounded
        Vector2 velocity = controller.data.currentVelocity;

        velocity.y = 0f;

        controller.data.currentVelocity = velocity;

        controller.data.isJumping = false;
        controller.data.isDoubleJumping = false;
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
            if (!Input.GetButtonDown("Jump") && !controller.data.isGrounded)
            {
                controller.movementLayer.SetState(new MovementAirborneState(controller));
            }
            else if (Input.GetButtonDown("Jump"))
            {
                controller.movementLayer.SetState(new MovementJumpState(controller));
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
