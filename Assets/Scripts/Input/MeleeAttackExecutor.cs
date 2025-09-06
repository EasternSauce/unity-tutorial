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
        ref Coroutine attackCoroutineRef,
        System.Action onAttackFinished)
    {
        if (!checkAttack()) return;

        if (attackCoroutineRef != null)
            StopCoroutine(attackCoroutineRef);

        attackCoroutineRef = StartCoroutine(MeleeAttackRoutine(command, attackAnimationTime,
            resetAttackTimer, setAnimationTimer, triggerAttackAnimation, attackCoroutineRef, onAttackFinished));
    }

    private IEnumerator MeleeAttackRoutine(Command command, float attackAnimationTime,
        System.Action resetAttackTimer,
        System.Action setAnimationTimer,
        System.Action triggerAttackAnimation,
        Coroutine attackCoroutineRef,
        System.Action onAttackFinished)
    {
        if (command.target == null)
        {
            onAttackFinished?.Invoke();
            yield break;
        }

        Transform targetTransform = command.target.transform;
        float attackBuffer = 0.1f;
        float range = attackRange;

        InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
        if (weapon == null || weapon.itemData.weaponType == WeaponType.None)
            range = 1.5f;

        while (command.target != null)
        {
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            if (distance <= range + attackBuffer)
            {
                StopMovement();
                RotateTowardsTarget(targetTransform, true);

                resetAttackTimer();
                setAnimationTimer();
                triggerAttackAnimation();

                yield return new WaitForSeconds(attackAnimationTime * 0.4f);

                if (command.target != null)
                {
                    IDamageable target = command.target.GetComponent<IDamageable>();
                    if (target != null)
                    {
                        int damage = character.GetDamage();
                        target.TakeDamage(damage);
                    }
                }

                command.isComplete = true;
                break;
            }
            else
            {
                Vector3 direction = (targetTransform.position - transform.position).normalized;
                Vector3 destination = targetTransform.position - direction * range;
                characterMovement.Agent.stoppingDistance = 0f;
                characterMovement.Agent.isStopped = false;
                characterMovement.SetDestination(destination);
            }
            yield return null;
        }

        characterMovement.Agent.stoppingDistance = characterMovement.DefaultStoppingDistance;
        attackCoroutineRef = null;
        onAttackFinished?.Invoke();
    }

    public override void ResetState()
    {
        base.ResetState();
    }
}
