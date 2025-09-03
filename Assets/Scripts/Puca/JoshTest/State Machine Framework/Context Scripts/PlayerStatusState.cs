using UnityEngine;
public abstract class PlayerStatusState : IState
{
    protected CharacterStateController controller;
    protected PlayerData data;
    protected Rigidbody2D rb;

    protected PlayerStatusState(CharacterStateController controller)
    {
        this.controller = controller;
        this.data = controller.data;
        this.rb = data.rb;
    }

    public virtual void EnterState()
    {
    }

    public virtual void ExitState()
    {
    }

    public virtual void Update()
    {
    }
    public virtual void FixedUpdate()
    {
    }
}
