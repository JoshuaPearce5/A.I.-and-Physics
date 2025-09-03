using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormDefaultState : PlayerFormState
{
    public FormDefaultState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Entered Default Form State");
        controller.animator.runtimeAnimatorController = data.defaultAnimator;
        controller.data.defaultRig.SetActive(true);
    }
    public override void ExitState()
    {
        controller.data.smokeEffect.Play();
        controller.data.defaultRig.SetActive(false);
    }
    public override void Update()
    {
        controller.energy.RegainEnergy(5f);

        if (Input.GetButtonDown("Fly") && controller.data.eagleFormUnlocked && !controller.data.isGrounded && controller.energy.currentEnergy > 5f)
        {
            controller.formLayer.SetState(new FormEagleState(controller));
            controller.movementLayer.SetState(new MovementFlyingState(controller));
        }
    }
    public override void FixedUpdate()
    {
        return;
    }
}