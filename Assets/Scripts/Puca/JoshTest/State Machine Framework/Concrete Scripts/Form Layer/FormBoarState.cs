using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormBoarState : PlayerFormState
{
    public FormBoarState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        controller.animator.runtimeAnimatorController = data.boarAnimator;
        controller.data.boarRig.SetActive(true);
    }
    public override void ExitState()
    {
        controller.data.smokeEffect.Play();
        controller.data.boarRig.SetActive(false);
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
