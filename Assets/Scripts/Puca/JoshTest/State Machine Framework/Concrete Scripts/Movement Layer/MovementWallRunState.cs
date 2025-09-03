using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MovementWallRunState : PlayerMovementState
{
    public float jumpCoyoteTime = 0.1f;
    public float jumpCoyoteCounter = 0f;
    public MovementWallRunState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Entering Wall Run State");
        // Player is performing a jump
        controller.data.isJumping = true;

        jumpCoyoteCounter = jumpCoyoteTime;

        controller.animator.SetBool("isGrounded", false);
        controller.animator.SetBool("isMoving", false);
        controller.animator.SetBool("isFalling", false);
        controller.animator.SetBool("isJumping", true);
        controller.animator.SetBool("isDoubleJumping", false);
        controller.animator.SetBool("isWallHanging", false);

        SoundFXManager.instance.PlayRandomPitchPlayerSoundFXClip(controller.data.jumpSoundClip, 1f);

        Vector2 velocity = controller.data.currentVelocity;

        velocity.x = 0f;
        velocity.y = controller.data.jumpForce;
        controller.data.currentVelocity = velocity;

        // Initialize jump hold counter
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

        if ((Input.GetButtonUp("Jump") || controller.data.jumpHeldCounter <= 0f) && !controller.data.isGrounded)
        {
            controller.movementLayer.SetState(new MovementAirborneState(controller));
        }
        else if ((Input.GetButtonUp("Jump") || controller.data.jumpHeldCounter <= 0f) && controller.data.isTouchingWall && controller.data.isAirborne)
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
}
