using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.RuleTile.TilingRuleOutput;
public class MovementWallHangState : PlayerMovementState
{
    public MovementWallHangState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Entering Wall Hang State");

        controller.data.isJumping = false;
        controller.data.isDoubleJumping = false;

        if (!controller.data.isFacingWall)
        {
            controller.data.defaultRig.GetComponent<SpriteRenderer>().flipX = !controller.data.defaultRig.GetComponent<SpriteRenderer>().flipX;
            controller.data.isFlipped = !controller.data.isFlipped;
        }
        controller.animator.SetBool("isGrounded", false);
        controller.animator.SetBool("isMoving", false);
        controller.animator.SetBool("isFalling", true);
        controller.animator.SetBool("isJumping", false);
        controller.animator.SetBool("isDoubleJumping", false);
        controller.animator.SetBool("isWallHanging", true);

        Vector2 velocity = controller.data.currentVelocity;

        velocity.x = 0f;
        velocity.y = 0f;

        controller.data.currentVelocity = velocity;
    }
    public override void ExitState()
    {
    }
    public override void Update()
    {
        // State Transitions
        if ((Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f || controller.data.isFacingWall) && controller.data.isGrounded)
        {
            controller.movementLayer.SetState(new MovementIdleState(controller));
        }

        if (!controller.data.controlsDisabled)
        {
            if (!controller.data.isFacingWall && controller.data.isAirborne && !Input.GetButton("Jump"))
            {
                controller.movementLayer.SetState(new MovementAirborneState(controller));
            }
            if (controller.data.isTouchingWall)
            {
                if (!Input.GetButton("Jump"))
                {
                    controller.data.hasReleasedJump = true;
                }

                if (controller.data.jumpBufferCounter > 0f && controller.data.hasReleasedJump && Input.GetButtonDown("Jump"))
                {
                    controller.data.hasReleasedJump = false;
                    controller.data.jumpBufferCounter = 0f;
                    controller.movementLayer.SetState(new MovementWallJumpState(controller));
                }
            }
        }
    }
    public override void FixedUpdate()
    {
        ApplyGravity();
    }

    public void ApplyGravity()
    {
        Vector2 velocity = controller.data.currentVelocity;

        if (!controller.data.isGrounded)
        {

            velocity.y += controller.data.gravityScale/10;
        }
        else if (velocity.y < 0)
        {
            velocity.y = 0;
        }

        controller.data.currentVelocity.y = velocity.y;
    }
}
