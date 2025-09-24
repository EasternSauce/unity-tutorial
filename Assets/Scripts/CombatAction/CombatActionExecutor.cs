using UnityEngine;

public abstract class CombatActionExecutor
{
    protected Character character;
    protected MoveCommandHandler movement;
    protected Animator animator;

    protected CombatActionExecutor(Character character, MoveCommandHandler movement, Animator animator)
    {
        this.character = character;
        this.movement = movement;
        this.animator = animator;
    }

    public abstract void Execute(Command command);
    public virtual void TickUpdate() { }
    public virtual void ResetState() { SetPerformingCombatAction(false); }
    protected abstract float ApplyCooldown(float baseCooldown);

    protected void StopMovement()
    {
        if (movement != null)
        {
            movement.Stop();
            if (movement.Agent != null && movement.Agent.enabled && movement.Agent.isOnNavMesh)
                movement.Agent.isStopped = true;
        }
    }

    protected void ResumeMovement()
    {
        if (movement != null && movement.Agent != null && movement.Agent.enabled && movement.Agent.isOnNavMesh)
            movement.Agent.isStopped = false;
    }

    protected void RotateTowardsPoint(Vector3 point)
    {
        Vector3 look = point - character.transform.position;
        look.y = 0f;
        if (look == Vector3.zero) return;
        character.transform.rotation = Quaternion.LookRotation(look);
    }

    protected bool AnimatorHasTrigger(string name)
    {
        if (animator == null) return false;
        foreach (var p in animator.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger && p.name == name)
                return true;
        return false;
    }

    protected void ResetAnimatorTriggers(params string[] triggers)
    {
        foreach (var t in triggers)
            if (AnimatorHasTrigger(t))
                animator.ResetTrigger(t);
    }

    protected void SetPerformingCombatAction(bool state)
    {
        if (character != null) character.isPerformingCombatAction = state;
    }
}
