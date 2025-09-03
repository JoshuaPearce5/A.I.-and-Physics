using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarState : PlayerState
{
    public BoarState(PlayerCharacterController controller) : base(controller) { }

    public override void EnterState()
    {
        controller.animator.runtimeAnimatorController = controller.data.boarAnimator;
        controller.data.boarRig.SetActive(true);
    }
    public override void ExitState()
    {
        controller.data.boarRig.SetActive(false);
    }
    public override void UpdateState()
    {
        // Switches to the Default State
        if (!Input.GetButton("Charge"))
        {
            controller.SwitchState(controller.defaultState);
            return;
        }
        // Switches to the Eagle State
        if (Input.GetButton("Fly"))
        {
            controller.SwitchState(controller.eagleState);
            return;
        }
    }

    public override void FixedUpdateState()
    {
        return;
    }
}
