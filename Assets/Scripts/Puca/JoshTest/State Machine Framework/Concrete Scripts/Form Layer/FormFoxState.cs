using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormFoxState : PlayerFormState
{
    public FormFoxState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        controller.animator.runtimeAnimatorController = data.foxAnimator;
        controller.data.foxRig.SetActive(true);
    }
    public override void ExitState()
    {
        controller.data.smokeEffect.Play();
        controller.data.foxRig.SetActive(false);
    }
    public override void Update()
    {
        return;
    }
    public override void FixedUpdate()
    {
        return;
    }
}
