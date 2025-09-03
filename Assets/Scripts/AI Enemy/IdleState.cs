using TMPro;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class IdleState : EnemyState
{
    public IdleState(EnemyController controller) : base(controller) { }

    public override void EnterState()
    {
        return;
    }

    public override void ExitState() 
    { 
        return;
    }

    public override void UpdateState()
    {
        if (controller.data.playerDetected)
        {
            controller.SwitchState(controller.preChargeState);
        }
    }

    public override void FixedUpdateState()
    {
        Hover();
    }

    public void Hover()
    {
        Vector3 targetPosition = controller.data.enemyOrigin.transform.position;

        float hoverFrequency = controller.data.hoverFrequency;
        float hoverAmplitude = controller.data.hoverAmplitude;

        controller.data.hoverTimer += Time.deltaTime * hoverFrequency;

        float bobX = Mathf.Sin(controller.data.hoverTimer) * hoverAmplitude;
        float bobY = Mathf.Cos(controller.data.hoverTimer) * hoverAmplitude;

        controller.transform.position = targetPosition + new Vector3(bobX, bobY, 0);

        if (bobX > 0)
        {
            controller.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (bobX < 0)
        {
            controller.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
