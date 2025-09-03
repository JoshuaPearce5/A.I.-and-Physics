using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxState : PlayerState
{
    public FoxState(PlayerCharacterController controller) : base(controller) { }

    public override void EnterState()
    {
        controller.animator.runtimeAnimatorController = controller.data.foxAnimator;
        controller.data.foxRig.SetActive(true);
    }
    public override void ExitState()
    {
        controller.data.foxRig.SetActive(false);
    }
    public override void UpdateState()
    {
        // Switches to the Default State
        if (controller.data.isAirborne || !Input.GetButton("Sneak"))
        {
            controller.SwitchState(controller.defaultState);
            return;
        }
    }

    public override void FixedUpdateState()
    {
        return;
    }
}
