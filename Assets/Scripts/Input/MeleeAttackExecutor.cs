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

        if (attackCoroutineRef != null)
            StopCoroutine(attackCoroutineRef);

        attackCoroutineRef = StartCoroutine(MeleeAttackRoutine(command, attackAnimationTime, resetAttackTimer, setAnimationTimer, triggerAttackAnimation));
        attackCoroutine = attackCoroutineRef;
    }

    private IEnumerator MeleeAttackRoutine(Command command, float attackAnimationTime,
        System.Action resetAttackTimer,
        System.Action setAnimationTimer,
        System.Action triggerAttackAnimation)
    {
        Transform targetTransform = command.target.transform;
        float attackBuffer = 0.1f;

        while (command.target != null)
        {
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            if (distance <= attackRange + attackBuffer)
            {
                StopMovement();
                RotateTowardsTarget(targetTransform, true);

                resetAttackTimer();
                setAnimationTimer();
                triggerAttackAnimation();

                yield return new WaitForSeconds(attackAnimationTime * 0.4f);

                if (command.target == null)
                    break;

                float currentDistance = Vector3.Distance(transform.position, command.target.transform.position);
                if (currentDistance <= attackRange + attackBuffer)
                {
                    IDamageable target = command.target.GetComponent<IDamageable>();
                    int damage = character.GetDamage();
                    target.TakeDamage(damage);
                }

                command.isComplete = true;
                break;
            }
            else
            {
                Vector3 direction = (targetTransform.position - transform.position).normalized;
                Vector3 destination = targetTransform.position - direction * attackRange;
                characterMovement.Agent.stoppingDistance = 0f;
                characterMovement.Agent.isStopped = false;
                characterMovement.SetDestination(destination);
            }
            yield return null;
        }

        characterMovement.Agent.stoppingDistance = characterMovement.DefaultStoppingDistance;
        attackCoroutine = null;
    }

    public override void ResetState()
    {
        base.ResetState();
    }
}
