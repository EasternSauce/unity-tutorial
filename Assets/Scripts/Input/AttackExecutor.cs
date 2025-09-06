using System.Collections;
using UnityEngine;

public abstract class AttackExecutor : MonoBehaviour
{
    protected Character character;
    protected CharacterMovement characterMovement;
    protected Animator animator;
    protected Coroutine attackCoroutine;

    protected virtual void Awake()
    {
        character = GetComponent<Character>();
        characterMovement = GetComponent<CharacterMovement>();
        animator = GetComponentInChildren<Animator>();
    }

    protected void StopMovement()
    {
        characterMovement.Stop();
        if (characterMovement.Agent != null)
            characterMovement.Agent.isStopped = true;
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
        bool isMoving = characterMovement.Agent.velocity.magnitude > 0.1f;

        if (forceInstant || isMoving)
            transform.rotation = targetRotation;
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3f * Time.deltaTime);
    }

    public virtual void ResetState()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }
}
