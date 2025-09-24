using UnityEngine;

public abstract class CombatActionExecutor : MonoBehaviour
{
    protected Character character;
    protected MoveCommandHandler characterMovement;
    protected Animator animator;

    protected virtual void Awake()
    {
        character = GetComponent<Character>();
        characterMovement = GetComponent<MoveCommandHandler>();
        animator = GetComponentInChildren<Animator>();
    }

    public abstract void Execute(Command command);

    protected void StopMovement()
    {
        if (characterMovement != null)
        {
            characterMovement.Stop();
            if (characterMovement.Agent != null && characterMovement.Agent.enabled && characterMovement.Agent.isOnNavMesh)
                characterMovement.Agent.isStopped = true;
        }
    }

    protected void RotateTowardsPoint(Vector3 point)
    {
        Vector3 lookVector = point - transform.position;
        lookVector.y = 0f;
        if (lookVector == Vector3.zero) return;
        transform.rotation = Quaternion.LookRotation(lookVector);
    }

    protected void RotateTowardsTarget(Transform target, bool forceInstant = false)
    {
        if (target == null) return;
        Vector3 lookVector = target.position - transform.position;
        lookVector.y = 0f;
        if (lookVector == Vector3.zero) return;
        Quaternion targetRotation = Quaternion.LookRotation(lookVector);
        bool isMoving = characterMovement.Agent != null && characterMovement.Agent.velocity.magnitude > 0.1f;
        if (forceInstant || isMoving) transform.rotation = targetRotation;
        else transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3f * Time.deltaTime);
    }

    protected void SetPerformingCombatAction(bool state)
    {
        if (character != null) character.isPerformingCombatAction = state;
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

    protected abstract float ApplyCooldown(float baseCooldown);

    public virtual void ResetState()
    {
        SetPerformingCombatAction(false);
    }
}
