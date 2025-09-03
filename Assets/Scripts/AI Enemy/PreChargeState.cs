using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PreChargeState : EnemyState
{
    public PreChargeState(EnemyController controller) : base(controller) { }

    private float elapsed;
    private float duration = 1f;   // how long it shakes before charging
    private float frequency = 40f;   // how fast it jitters
    private float amplitude = 0.3f;  // how wide the jitter is
    private Vector3 origin;

    public override void EnterState()
    {
        controller.sprite.color = Color.red;       // flash red as telegraph
        origin = controller.transform.position;    // save starting position
        elapsed = 0f;

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
        controller.transform.position = origin;    // ensure reset position
    }

    public override void UpdateState()
    {
        elapsed += Time.deltaTime;

        // Left-right jitter
        float offsetX = Mathf.Sin(elapsed * frequency) * amplitude;
        controller.transform.position = origin + new Vector3(offsetX, 0, 0);

        // Once timer is up → switch to Charge state
        if (elapsed >= duration)
        {
            controller.SwitchState(controller.chargingState);
        }
    }

    public override void FixedUpdateState()
    {
        // Not needed for PreCharge
    }
}

