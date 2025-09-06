using System.Collections;
using CharacterCommand;
using UnityEngine;

public class MeleeAttackExecutor : AttackExecutor
{
    [SerializeField] float attackRange = 2.5f;

    public void HandleMeleeAttack(Command command,
        float attackAnimationTime,
        System.Func<bool> checkAttack,
        System.Action resetAttackTimer,
        System.Action setAnimationTimer,
        System.Action triggerAttackAnimation,
        ref Coroutine attackCoroutineRef)
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

            if (attackCoroutineRef != null)
                StopCoroutine(attackCoroutineRef);

            attackCoroutineRef = StartCoroutine(DelayedDamage(command, attackAnimationTime));
            attackCoroutine = attackCoroutineRef;
        }
        else
        {
            Vector3 direction = (targetTransform.position - transform.position).normalized;
            Vector3 destination = targetTransform.position - direction * attackRange;

            characterMovement.Agent.stoppingDistance = 0f;
            characterMovement.Agent.isStopped = false;
            characterMovement.SetDestination(destination);

            if (attackCoroutineRef != null)
            {
                StopCoroutine(attackCoroutineRef);
                attackCoroutineRef = null;
            }
        }
    }

    private IEnumerator DelayedDamage(Command command, float attackAnimationTime, float delay = -1f)
    {
        float hitTime = attackAnimationTime * 0.4f;
        if (delay >= 0f) hitTime = delay;

        yield return new WaitForSeconds(hitTime);

        if (attackCoroutine == null || command == null || command.isComplete || command.target == null)
            yield break;

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

        AttackHandler attackHandler = GetComponent<AttackHandler>();
        attackHandler?.ResetAttackTimer();

        attackCoroutine = null;
    }

    private void DealDamage(Command command)
    {
        IDamageable target = command.target.GetComponent<IDamageable>();
        int damage = character.GetDamage();
        target.TakeDamage(damage);
    }

    public override void ResetState()
    {
        base.ResetState();
    }
}
