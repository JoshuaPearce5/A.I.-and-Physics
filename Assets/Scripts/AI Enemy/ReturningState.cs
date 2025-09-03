using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturningState : EnemyState
{
    private Vector3 targetPosition;
    private float speed = 5f;            // movement speed
    private float stopDistance = 0.2f;   // how close to origin before stopping

    public ReturningState(EnemyController controller) : base(controller) { }

    public override void EnterState()
    {
        targetPosition = controller.data.enemyOrigin.transform.position;

        if ((targetPosition.x - controller.transform.position.x) > 0)
        {
            controller.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if ((targetPosition.x - controller.transform.position.x) < 0)
        {
            controller.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public override void ExitState()
    {
        // optional: cleanup
    }

    public override void UpdateState()
    {
        // Calculate direction towards origin
        Vector3 direction = (targetPosition - controller.transform.position);
        float distance = direction.magnitude;

        if (distance <= stopDistance)
        {
            // Close enough → go idle
            controller.SwitchState(controller.idleState);
            return;
        }

        // Move towards origin
        direction.Normalize();
        controller.transform.position += direction * speed * Time.deltaTime;
    }

    public override void FixedUpdateState()
    {
        // Not needed unless you want physics-based movement
    }
}

