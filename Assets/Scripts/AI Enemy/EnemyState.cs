using UnityEngine;

public abstract class EnemyState
{
    protected EnemyController controller;

    public EnemyState(EnemyController controller)
    {
        this.controller = controller;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}
