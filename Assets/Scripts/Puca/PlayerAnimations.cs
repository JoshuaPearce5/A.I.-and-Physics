using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] CharacterStateController controller;

    private void Update()
    {
        UpdateAnimationStates();
    }

    private void UpdateAnimationStates()
    {
        if (controller != null)
        {
            // Handles animation transitions based on player logic booleans
            controller.animator.SetBool("isGrounded", controller.data.isGrounded);
            controller.animator.SetBool("isFalling", !Input.GetButtonDown("Jump") && !controller.data.isGrounded);

            if (!controller.data.controlsDisabled)
            {
                controller.animator.SetBool("isJumping", controller.data.jumpHeld);
                controller.animator.SetBool("isDoubleJumping", controller.data.isDoubleJumping);
                controller.animator.SetBool("isWallHanging", controller.data.isTouchingWall && !controller.data.isGrounded);
                controller.animator.SetBool("isMoving", Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f && !controller.data.isFacingWall && controller.data.isGrounded);
            }
        }
    }
}
