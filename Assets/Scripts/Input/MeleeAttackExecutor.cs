using System.Collections;
using CharacterCommand;
using UnityEngine;

public class MeleeAttackExecutor : AttackExecutor
{
    [Header("Melee Settings")]
    [SerializeField] float attackRange = 2.5f;

    public void HandleMeleeAttack(Command command,
        float attackAnimationTime,
        System.Func<bool> checkAttack,
        System.Action resetAttackTimer,
        System.Action setAnimationTimer,
        System.Action triggerAttackAnimation,
        ref Coroutine attackCoroutine)
    {
        if (!checkAttack()) return;

        if (command.target == null)
        {
            StopMovement();

            resetAttackTimer();
            setAnimationTimer();
            triggerAttackAnimation();

            command.isComplete = true;
            return;
        }

        float distance = Vector3.Distance(transform.position, command.target.transform.position);
        float attackBuffer = 0.1f;
        Transform targetTransform = command.target.transform;

        RotateTowardsTarget(targetTransform);

        if (distance <= attackRange + attackBuffer)
        {
            StopMovement();

            if (!checkAttack()) return;

            RotateTowardsTarget(targetTransform, true);

            resetAttackTimer();
            setAnimationTimer();
            triggerAttackAnimation();

            if (attackCoroutine != null)
                StopCoroutine(attackCoroutine);

            attackCoroutine = StartCoroutine(DelayedDamage(command, attackAnimationTime));
        }
        else
        {
            Vector3 direction = (targetTransform.position - transform.position).normalized;
            Vector3 destination = targetTransform.position - direction * attackRange;

            characterMovement.Agent.stoppingDistance = 0f;
            characterMovement.Agent.isStopped = false;
            characterMovement.SetDestination(destination);

            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private IEnumerator DelayedDamage(Command command, float attackAnimationTime, float delay = -1f)
    {
        float hitTime = attackAnimationTime * 0.4f;
        if (delay >= 0f) hitTime = delay;

        yield return new WaitForSeconds(hitTime);

        if (command == null || command.isComplete || command.target == null)
        {
            characterMovement.Agent.stoppingDistance = characterMovement.DefaultStoppingDistance;
            yield break;
        }

        float currentDistance = Vector3.Distance(transform.position, command.target.transform.position);
        float attackBuffer = 0.1f;

        if (currentDistance > attackRange + attackBuffer)
        {
            command.isComplete = true;
            characterMovement.Agent.stoppingDistance = characterMovement.DefaultStoppingDistance;
            yield break;
        }

        DealDamage(command);
        command.isComplete = true;
        characterMovement.Agent.stoppingDistance = characterMovement.DefaultStoppingDistance;
        attackCoroutine = null;
    }

    private void DealDamage(Command command)
    {
        IDamageable target = command.target.GetComponent<IDamageable>();
        int damage = character.GetDamage();
        target.TakeDamage(damage);
    }
}
