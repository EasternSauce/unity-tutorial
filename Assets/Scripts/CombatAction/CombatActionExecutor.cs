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
    public abstract void TickUpdate();
    protected abstract void ResetState();
    protected abstract float ApplyCooldown(float baseCooldown);

    public void CancelOngoingCombatAction()
    {
        if (character != null && HasActiveTarget())
        {
            ResetState();
        }
    }

    protected virtual bool HasActiveTarget()
    {
        return false;
    }

    protected void StopMovement()
    {
        movement?.Stop();
    }

    protected void ResumeMovement()
    {
        movement?.MoveTo(character.transform.position);
    }

    protected void FaceDirection(Vector3 point)
    {
        if (character == null) return;
        Vector3 direction = point - character.transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
            character.transform.forward = direction.normalized;
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
}
