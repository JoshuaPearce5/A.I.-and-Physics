using UnityEngine;

public abstract class PlayerState
{
    protected PlayerCharacterController controller;

    public PlayerState(PlayerCharacterController controller)
    {
        this.controller = controller;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}