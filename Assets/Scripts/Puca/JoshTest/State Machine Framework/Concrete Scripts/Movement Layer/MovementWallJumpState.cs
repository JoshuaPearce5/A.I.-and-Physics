using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MovementWallJumpState : PlayerMovementState
{
    public float jumpCoyoteTime = 0.1f;
    public float jumpCoyoteCounter = 0f;
    public MovementWallJumpState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Entering Wall Jump State");
        // Player is jumping
        controller.data.isJumping = true;

        controller.data.isFlipped = !controller.data.isFlipped;

        float wallSide = controller.data.isFlipped ? -1f : 1f;

        controller.transform.position += Vector3.right * wallSide * 0.05f;

        jumpCoyoteCounter = jumpCoyoteTime;

        controller.animator.SetBool("isGrounded", false);
        controller.animator.SetBool("isMoving", false);
        controller.animator.SetBool("isFalling", false);
        controller.animator.SetBool("isJumping", true);
        controller.animator.SetBool("isDoubleJumping", false);
        controller.animator.SetBool("isWallHanging", false);

        Vector2 velocity = controller.data.currentVelocity;

        velocity.x = wallSide * controller.data.wallJumpForceX;

        velocity.y = controller.data.wallJumpForceY;

        controller.data.wallJumpLockCounter = controller.data.wallJumpTime;

        controller.data.defaultRig.GetComponent<SpriteRenderer>().flipX = !controller.data.defaultRig.GetComponent<SpriteRenderer>().flipX;

        controller.data.isFacingWall = false;

        controller.data.currentVelocity = velocity;

        controller.data.jumpHeldCounter = controller.data.maxJumpTime;
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

        if (controller.data.wallJumpLockCounter > 0f)
        {
            controller.data.wallJumpLockCounter -= Time.deltaTime;
        }

        if (jumpCoyoteCounter <= 0f)
        {
            if ((Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f || controller.data.isFacingWall) && controller.data.isGrounded && !controller.data.jumpHeld)
            {
                controller.movementLayer.SetState(new MovementIdleState(controller));
            }

            if (!controller.data.controlsDisabled)
            {
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f && controller.data.isGrounded)
                {
                    controller.movementLayer.SetState(new MovementRunState(controller));
                }
            }
        }

        if ((Input.GetButtonUp("Jump") || controller.data.jumpHeldCounter <= 0f || jumpCoyoteCounter <= 0) && !controller.data.isGrounded)
        {
            controller.movementLayer.SetState(new MovementAirborneState(controller));
        }
        else if (controller.data.isFacingWall && controller.data.isAirborne)
        {
            controller.movementLayer.SetState(new MovementWallHangState(controller));
        }
    }
    public override void FixedUpdate()
    {
        ApplyMovement();
    }
    public void ApplyMovement()
    {
        if (controller.data.wallJumpLockCounter > 0f)
        {
            // Decrease lock counter
            controller.data.wallJumpLockCounter -= Time.fixedDeltaTime;
        }
        else if (controller.data.wallJumpLockCounter < 0.1f && controller.data.moveDirection != 0)
        {
            Vector2 velocity = controller.data.currentVelocity;

            float targetSpeed = controller.data.moveDirection * controller.data.maxRunSpeed;
            float speedDiff = targetSpeed - velocity.x;
            float accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? controller.data.runAcceleration : controller.data.runDeceleration;

            // Apply acceleration
            velocity.x += speedDiff * accelerationRate;

            if (controller.data.isTouchingCeiling)
            {
                velocity.y = 0f;
                controller.data.currentVelocity.y = velocity.y;

            }

            // Clamp velocity.x to maxRunSpeed
            velocity.x = Mathf.Clamp(velocity.x, -controller.data.maxRunSpeed, controller.data.maxRunSpeed);
            controller.data.currentVelocity.x = velocity.x;
        }
    }
}
