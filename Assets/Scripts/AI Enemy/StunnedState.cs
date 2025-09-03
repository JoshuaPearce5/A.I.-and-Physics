using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : EnemyState
{
    private float elapsed;
    private float stunDuration = 2f;      // total stun time
    private float flashFrequency = 10f;   // how fast the sprite flashes
    private Color color1 = Color.yellow;
    private Color color2 = Color.white;

    public StunnedState(EnemyController controller) : base(controller) { }

    public override void EnterState()
    {
        elapsed = 0f;
        controller.sprite.color = color2;  // start flash color

        // Direction away from the ground contact
        //Vector3 knockbackDir = (controller.transform.position.normalized - controller.groundLocation.normalized);

        //float knockbackDistance = 3f;  // tweak for how far enemy is knocked back
        //controller.transform.position += knockbackDir * knockbackDistance;
    }


    public override void ExitState()
    {
        controller.sprite.color = Color.white; // reset color
    }

    public override void UpdateState()
    {
        elapsed += Time.deltaTime;

        // Flash between yellow and white
        float t = Mathf.PingPong(elapsed * flashFrequency, 1f);
        controller.sprite.color = Color.Lerp(color2, color1, t);

        // Stun duration finished
        if (elapsed >= stunDuration)
        {
            if (controller.data.playerDetected)
            {
                controller.SwitchState(controller.preChargeState);
            }
            else
            {
                controller.SwitchState(controller.returningState);
            }
        }
    }

    public override void FixedUpdateState()
    {
        // Nothing needed here for stun
    }
}

