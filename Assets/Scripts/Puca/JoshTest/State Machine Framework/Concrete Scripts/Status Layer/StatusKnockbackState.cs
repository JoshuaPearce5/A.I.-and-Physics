using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusKnockbackState : PlayerStatusState
{
    public StatusKnockbackState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Player Entered Knockback State");
        controller.data.isKnockedBack = true;
        controller.data.knockbackCounter = controller.data.knockbackTime;
    }
    public override void ExitState()
    {
        controller.data.isKnockedBack = false;
    }
    public override void Update()
    {
        controller.data.knockbackCounter -= 0.1f * Time.deltaTime;

        if (controller.data.knockbackCounter <= 0)
        {
            controller.statusLayer.SetState(new StatusDefaultState(controller));
        }
    }
    public override void FixedUpdate()
    {
        return;
    }
}
