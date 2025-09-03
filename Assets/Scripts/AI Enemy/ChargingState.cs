using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingState : EnemyState
{
    public ChargingState(EnemyController controller) : base(controller) { }

    private Vector3 origin;
    private Vector3 target;
    private Vector3 direction;
    private float distanceTravelled;

    private float maxDistance = 8f;

    public override void EnterState()
    {
        origin = controller.transform.position;
        target = controller.playerLocation;   // player position at start
        direction = (target - origin).normalized;        // charge direction
        distanceTravelled = 0f;

        if ((controller.playerLocation.x - controller.transform.position.x) > 0)
        {
            controller.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if ((controller.playerLocation.x - controller.transform.position.x) < 0)
        {
            controller.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public override void ExitState()
    {
        // optional: cleanup or reset
    }

    public override void UpdateState()
    {
        if (distanceTravelled >= maxDistance)
        {
            controller.SwitchState(controller.stunnedState);
        }
    }

    public override void FixedUpdateState()
    {
        float chargeSpeed = controller.data.chargeSpeed;
        // Move in fixed steps for physics consistency
        float step = chargeSpeed * Time.fixedDeltaTime;
        controller.transform.position += direction * step;

        distanceTravelled += step;
    }
}

