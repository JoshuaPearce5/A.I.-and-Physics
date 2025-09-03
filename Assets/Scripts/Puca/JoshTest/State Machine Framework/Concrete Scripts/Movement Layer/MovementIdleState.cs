using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MovementIdleState : PlayerMovementState
{
    public MovementIdleState(CharacterStateController controller) : base(controller)
    {
    }

    public override void EnterState()
    {
        Debug.Log("Entering Idle State");
        controller.animator.SetBool("isGrounded", true);
        controller.animator.SetBool("isMoving", false);
        controller.animator.SetBool("isFalling", false);
        controller.animator.SetBool("isJumping", false);
        controller.animator.SetBool("isDoubleJumping", false);
        controller.animator.SetBool("isWallHanging", false);

        // Reset vertical velocity when grounded
        Vector2 velocity = controller.data.currentVelocity;

        velocity.x = 0f;
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
        if (!controller.data.controlsDisabled)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f && !controller.data.isFacingWall && controller.data.isGrounded)
            {
                controller.movementLayer.SetState(new MovementRunState(controller));
            }
            else if (!Input.GetButtonDown("Jump") && !controller.data.isGrounded)
            {
                controller.movementLayer.SetState(new MovementAirborneState(controller));
            }
            else if (Input.GetButtonDown("Jump") && controller.data.isGrounded && !controller.data.isFacingWall)
            {
                controller.movementLayer.SetState(new MovementJumpState(controller));
            }
            else if (Input.GetButtonDown("Jump") && controller.data.isGrounded && controller.data.isFacingWall)
            {
                controller.movementLayer.SetState(new MovementWallRunState(controller));
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

            velocity.y += controller.data.gravityScale;
        }
        else if (velocity.y < 0)
        {
            velocity.y = 0;
        }

        controller.data.currentVelocity.y = velocity.y;
    }
}