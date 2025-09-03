using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DefaultState : PlayerState
{
    public DefaultState(PlayerCharacterController controller) : base(controller) { }
    public override void EnterState()
    {
        controller.animator.runtimeAnimatorController = controller.data.defaultAnimator;
        controller.data.defaultRig.SetActive(true);
    }
    public override void ExitState()
    {
        controller.data.smokeEffect.Play();
        controller.data.defaultRig.SetActive(false);
    }
    public override void UpdateState()
    {
        // Runs all custom functions for this State
        HandleWallJumpLock();
        HandleMovementInput();
        HandleFacingDirection();
        HandleJumpInput();
        RegenerateEnergy();
    }

    public override void FixedUpdateState()
    {
        // Runs Physics calculations for this State
        ApplyMovement();
        ApplyGravity();
    }

    public void ApplyMovement()
{
    // Handles movement physics
    Vector2 velocity = controller.data.rb.velocity;

    // Decrement the double jump control timer
    if (controller.data.doubleJumpControlCounter > 0f)
    {
        controller.data.doubleJumpControlCounter -= Time.fixedDeltaTime;
    }
    // If wallJumpLockCounter has hit 0 and player isn't Facing a wall
    if (controller.data.wallJumpLockCounter <= 0f && !controller.data.isFacingWall)
    {
        // If player is moving horizontally
        if (controller.data.moveDirection != 0)
        {
            // If the player is moving horizontally in the air before double jumping
            if (controller.data.isAirborne && !controller.data.isDoubleJumping)
            {
                // Apply moveDirection and moveSpeed to the player's velocity on the X-axis
                float targetVelocityX = controller.data.moveDirection * controller.data.moveSpeed;
                // Interpolate the X-axis velocity with a multiplier for less horizontal control in the air
                velocity.x = Mathf.Lerp(velocity.x, targetVelocityX, controller.data.airControlMultiplier * Time.fixedDeltaTime);
            }
            // If the player is moving horizontally in the air after double jumping
            else if (controller.data.isAirborne && controller.data.isDoubleJumping)
            {
                // Apply moveDirection and moveSpeed to the player's velocity on the X-axis
                float targetVelocityX = controller.data.moveDirection * controller.data.moveSpeed;

                // If the player has just double jumped
                if (controller.data.doubleJumpControlCounter > 0f)
                {
                    // Interpolate the X-axis velocity with no multiplier for more horizontal control after double jump
                    velocity.x = Mathf.Lerp(velocity.x, targetVelocityX, controller.data.doubleJumpControlMultiplier * Time.fixedDeltaTime);
                }
                // If the double jump control timer hits 0 the player should have less air control again
                else
                {
                    // Interpolate the X-axis velocity with a multiplier for less horizontal control in the air
                    velocity.x = Mathf.Lerp(velocity.x, targetVelocityX, controller.data.airControlMultiplier * Time.fixedDeltaTime);
                }
            }
            // If the player is moving horizontally and is not in the air
            else
            {
                // Apply moveDirection and moveSpeed to the player's velocity on the X-axis
                velocity.x = controller.data.moveDirection * controller.data.moveSpeed;
            }
        }
        // If the player is not moving horizontally and is not Facing the ground
        else if (controller.data.moveDirection == 0 && controller.data.isAirborne)
        {
            // Target X-axis velocity should be 0 if input is not being pressed
            float targetVelocityX = 0f;
            // Interpolate between current velocity x and zero to slow player based on slowSpeed factor
            velocity.x = Mathf.Lerp(velocity.x, targetVelocityX, controller.data.slowSpeed * Time.fixedDeltaTime);
        }
        // If the player is not moving horizontally and is Facing the ground
        else if (controller.data.moveDirection == 0 && !controller.data.isAirborne)
        {
            // Target X-axis velocity should be 0 if input is not being pressed
            float targetVelocityX = 0f;
            // Instantly stop the player's horizontal velocity
            velocity.x = targetVelocityX;
        }
    }
    controller.data.rb.velocity = velocity;
}

    public void ApplyGravity()
    {
        // Handles gravity physics
        if (controller.data.isAirborne)
        {
            // If Y-Axis velocity is greater than zero and Jump is not being held
            if (controller.data.rb.velocity.y > 0f && !controller.data.jumpHeld)
            {
                // Player gravity scale gets a jumping multiplier
                controller.data.rb.gravityScale = controller.data.jumpGravityMult;
            }
            // If Y-Axis velocity is less than or equal to zero
            else if (controller.data.rb.velocity.y <= 0f)
            {
                // Player gravity scale gets a falling multiplier
                controller.data.rb.gravityScale = controller.data.fallGravityMult;
                // Clamps player fall speed to prevent too much acceleration
                controller.data.rb.velocity = new Vector2(controller.data.rb.velocity.x, Mathf.Clamp(controller.data.rb.velocity.y, controller.data.maxFallSpeed, float.MaxValue));
            }
            else
            {
                // If none of the above is true, gravity scale is reset to one
                controller.data.rb.gravityScale = 1f;
            }
        }
        else
        {
            // If Player is not airborne, gravity scale is reset to one
            controller.data.rb.gravityScale = 1f;
        }
    }
    public void HandleFacingDirection()
    {
        // Change direction player is facing
        if (controller.data.moveDirection > 0f)
        {
            controller.transform.localScale = new Vector3(Mathf.Abs(controller.transform.localScale.x), controller.transform.localScale.y, controller.transform.localScale.z);
        }
        else if (controller.data.moveDirection < 0f)
        {
            controller.transform.localScale = new Vector3(-Mathf.Abs(controller.transform.localScale.x), controller.transform.localScale.y, controller.transform.localScale.z);
        }
    }
    public void RegenerateEnergy()
    {
        if (!Input.GetButton("Fly") && !Input.GetButton("Sneak") && !Input.GetButton("Charge"))
        {
            controller.energy.RegainEnergy(5f);
        }
    }
    public void HandleMovementInput()
    {
        // Controls player movement input
        controller.data.moveDirection = (controller.data.wallJumpLockCounter <= 0f) ? Input.GetAxisRaw("Horizontal") : 0f;
        controller.data.jumpHeld = Input.GetButton("Jump");
    }
    public void HandleJumpInput()
    {
        if ((controller.data.isJumping || controller.data.isDoubleJumping) && controller.data.jumpHeld && controller.data.jumpHeldCounter > 0f && !Input.GetButtonDown("Jump"))
        {
            // Handle variable jump height logic after jump has been initiated
            controller.data.rb.velocity = new Vector2(controller.data.rb.velocity.x, controller.data.jumpForce);
            controller.data.jumpHeldCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            controller.data.jumpPressed = true;
            ExecuteJump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            controller.data.jumpHeld = false;
        }
    }
    public void HandleWallJumpLock()
    {
        // Countdown for lockup of controls after wall jump
        if (controller.data.wallJumpLockCounter > 0)
        {
            controller.data.wallJumpLockCounter -= Time.deltaTime;
            controller.data.moveDirection = 0f;
        }
    }
    public void ExecuteJump()
    {
        if (controller.data.jumpHeldCounter == 0)
        {
            controller.data.jumpPressed = false;
        }
        // Player is grounded
        if (controller.data.isGrounded)
        {
            if (controller.data.isFacingWall)
            {
                // Grounded and Facing a wall — perform a wall run
                WallRun();
                SoundFXManager.instance.PlayRandomPitchPlayerSoundFXClip(controller.data.jumpSoundClip, 1f);
            }
            else
            {
                SoundFXManager.instance.PlayRandomPitchPlayerSoundFXClip(controller.data.jumpSoundClip, 1f);
                Jump();
            }
        }
        // Wall jump: airborne but Facing wall
        else if (controller.data.isFacingWall && controller.data.isAirborne)
        {
            SoundFXManager.instance.PlayRandomPitchPlayerSoundFXClip(controller.data.doubleJumpSoundClip, 1f);
            WallJump();
        }
        // Double jump
        else if (!controller.data.isGrounded && !controller.data.isDoubleJumping)
        {
            SoundFXManager.instance.PlayRandomPitchPlayerSoundFXClip(controller.data.doubleJumpSoundClip, 1f);
            DoubleJump();
        }
    }
    public void Jump()
    {
        Debug.Log("Jump!");
        // Player is performing a jump
        controller.data.isJumping = true;

        // Player is holding jump input
        controller.data.jumpHeld = true;

        // Apply the jump force
        controller.data.rb.velocity = new Vector2(controller.data.rb.velocity.x, controller.data.jumpForce);

        // Set the jump counter for held jumps
        controller.data.jumpHeldCounter = controller.data.maxJumpTime;
    }

    public void WallJump()
    {
        Debug.Log("WallJump!");
        // Player is jumping
        controller.data.isJumping = true;

        // Detect which side player is facing
        int wallSide = (int)Mathf.Sign(controller.transform.localScale.x);

        // Apply force to push the player away from the wall
        controller.data.rb.velocity = new Vector2(-wallSide * controller.data.wallJumpForceX, controller.data.wallJumpForceY);

        // Set wall jump input lock timer
        controller.data.wallJumpLockCounter = controller.data.wallJumpTime;

        // Change player facing direction
        controller.transform.localScale = new Vector3(Mathf.Sign(controller.data.rb.velocity.x) * Mathf.Abs(controller.transform.localScale.x), controller.transform.localScale.y, controller.transform.localScale.z);
    }

    public void WallRun()
    {
        Debug.Log("Wall Run!");
        // Player is performing a jump
        controller.data.isJumping = true;

        // Player is holding jump input
        controller.data.jumpHeld = true;

        // Set the jump counter for held jumps
        controller.data.jumpHeldCounter = controller.data.maxJumpTime;

        // Apply vertical wall run jump force
        controller.data.rb.velocity = new Vector2(controller.data.rb.velocity.x, controller.data.wallRunJumpForce);
    }

    public void DoubleJump()
    {
        Debug.Log("Double Jump!");
        // Player is performing a jump
        controller.data.isDoubleJumping = true;

        // Player is holding jump input
        controller.data.jumpHeld = true;

        // Apply the jump force
        controller.data.rb.velocity = new Vector2(controller.data.rb.velocity.x, controller.data.jumpForce);

        // Set the jump counter for held jumps
        controller.data.jumpHeldCounter = controller.data.maxJumpTime;

        // Set the double jump air control timer
        controller.data.doubleJumpControlCounter = controller.data.doubleJumpControlDuration;
    }
}