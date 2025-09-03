using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleState : PlayerState
{
    public EagleState(PlayerCharacterController controller) : base(controller) { }

    public override void EnterState()
    {
        SoundFXManager.instance.PlayRandomPitchPlayerSoundFXClip(controller.data.transformSoundClip, 0.75f);

        controller.animator.runtimeAnimatorController = controller.data.eagleAnimator;
        controller.data.eagleRig.SetActive(true);
        controller.data.rb.gravityScale = 0f;


    }
    public override void ExitState()
    {
        SoundFXManager.instance.PlayRandomPitchPlayerSoundFXClip(controller.data.transformSoundClip, 0.75f);

        controller.data.smokeEffect.Play();
        controller.data.eagleRig.SetActive(false);
    }

    public override void UpdateState()
    {
        // Switches to the Default State
        if (controller.data.isAirborne && !Input.GetButton("Fly") || controller.data.isGrounded || controller.data.isTouchingWall)
        {
            controller.SwitchState(controller.defaultState);
            return;
        }
        // Switches to the Boar State
        if (Input.GetButton("Charge"))
        {
            controller.SwitchState(controller.boarState);
            return;
        }

        HandleMovementInput();

        HandleFacingDirection();

        ApplyMovement();

        controller.energy.UseEnergy(30f);
    }

    public override void FixedUpdateState()
    {
        return;
    }

    private void HandleMovementInput()
    {
        // Controls player movement input
        controller.data.moveDirection = Input.GetAxisRaw("Horizontal");

        controller.data.verticalMoveDirection = Input.GetAxisRaw("Vertical");
    }

    private void HandleFacingDirection()
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
    
    private void ApplyMovement()
    {
        Vector2 velocity = controller.data.rb.velocity;

        if (controller.data.moveDirection < 0f)
        {
            float targetVelocityX = controller.data.flySpeed * -1f;

            velocity.x = Mathf.Lerp(velocity.x, targetVelocityX, controller.data.flySpeed * Time.fixedDeltaTime);

        }
        else if (controller.data.moveDirection > 0f)
        {
            float targetVelocityX = controller.data.flySpeed;

            velocity.x = Mathf.Lerp(velocity.x, targetVelocityX, controller.data.flySpeed * Time.fixedDeltaTime);

        }
        if (controller.data.verticalMoveDirection < 0f)
        {
            float targetVelocityY = controller.data.downwardFlySpeed;

            velocity.y = Mathf.Lerp(velocity.y, targetVelocityY, controller.data.flySpeed * Time.fixedDeltaTime);

        }
        else if (controller.data.verticalMoveDirection > 0f)
        {
            float targetVelocityY = controller.data.upwardFlySpeed;

            velocity.y = Mathf.Lerp(velocity.y, targetVelocityY, controller.data.flySpeed * Time.fixedDeltaTime);
        }



        controller.data.rb.velocity = velocity;
    }
}