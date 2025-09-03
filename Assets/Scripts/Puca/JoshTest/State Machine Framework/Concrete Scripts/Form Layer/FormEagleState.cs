using UnityEngine;

public class FormEagleState : PlayerFormState
{
    public FormEagleState(CharacterStateController controller) : base(controller)
    {
    }
    public override void EnterState()
    {
        Debug.Log("Entered Eagle Form State");
        controller.animator.runtimeAnimatorController = data.eagleAnimator;
        controller.data.eagleRig.SetActive(true);
        controller.data.eagleFormActive = true;
    }
    public override void ExitState()
    {
        controller.data.smokeEffect.Play();
        controller.data.eagleRig.SetActive(false);
        controller.data.eagleFormActive = false;
    }
    public override void Update()
    {
        controller.energy.UseEnergy(30f);

        if (controller.energy.currentEnergy <= 0 || Input.GetButtonUp("Fly"))
        {
            controller.formLayer.SetState(new FormDefaultState(controller));
        }
    }
    public override void FixedUpdate()
    {
        return;
    }
}
