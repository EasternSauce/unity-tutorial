using System.Collections;
using CharacterCommand;
using UnityEngine;

public class MeleeAttackExecutor : AttackExecutor
{
    [SerializeField] float attackRange = 2.5f;
    [SerializeField] float defaultTimeToAttack = 1f;
    [SerializeField] float attackAnimationTime = 1f;

    private float attackTimer;
    private Coroutine localCoroutine;

    private void Update()
    {
        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    public void HandleMeleeAttack(Command command)
    {
        if (attackTimer > 0f) return;
        if (command.target == null) return;

        if (localCoroutine != null) StopCoroutine(localCoroutine);
        localCoroutine = StartCoroutine(MeleeAttackRoutine(command));
    }

    private IEnumerator MeleeAttackRoutine(Command command)
    {
        Transform targetTransform = command.target.transform;

        while (command.target != null)
        {
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            float range = attackRange;

            InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
            if (weapon == null || weapon.itemData.weaponType == WeaponType.None)
                range = 1.5f;

            if (distance <= range + 0.1f)
            {
                StopMovement();
                RotateTowardsTarget(targetTransform, true);
                TriggerAttackAnimation();
                ResetAttackTimer();

                yield return new WaitForSeconds(attackAnimationTime * 0.4f);

                IDamageable target = command.target.GetComponent<IDamageable>();
                if (target != null)
                {
                    int damage = character.GetDamage();
                    target.TakeDamage(damage);
                }

                command.isComplete = true;
                break;
            }
            else
            {
                Vector3 dir = (targetTransform.position - transform.position).normalized;
                Vector3 destination = targetTransform.position - dir * range;
                characterMovement.Agent.stoppingDistance = 0f;
                characterMovement.Agent.isStopped = false;
                characterMovement.SetDestination(destination);
            }
            yield return null;
        }

        characterMovement.Agent.stoppingDistance = characterMovement.DefaultStoppingDistance;
        localCoroutine = null;
    }

    private void ResetAttackTimer()
    {
        float atkSpeed = character.GetStatsValue(Statistic.AttackSpeed).float_value;
        attackTimer = defaultTimeToAttack / atkSpeed;
    }

    private void TriggerAttackAnimation()
    {
        InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
        WeaponType type = weapon != null ? weapon.itemData.weaponType : WeaponType.None;

        string trigger = null;

        if (type == WeaponType.OneHandedAxe && AnimatorHasTrigger("OneHandedMeleeAttack"))
            trigger = "OneHandedMeleeAttack";
        else if (type == WeaponType.TwoHandedAxe && AnimatorHasTrigger("TwoHandedMeleeAttack"))
            trigger = "TwoHandedMeleeAttack";

        if (string.IsNullOrEmpty(trigger))
        {
            if (AnimatorHasTrigger("Attack"))
                trigger = "Attack";
            else if (AnimatorHasTrigger("FistAttack"))
                trigger = "FistAttack";
        }

        if (!string.IsNullOrEmpty(trigger))
            animator.SetTrigger(trigger);
    }

    private bool AnimatorHasTrigger(string name)
    {
        if (animator == null) return false;
        foreach (var p in animator.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger && p.name == name)
                return true;
        return false;
    }

    public override void ResetState()
    {
        base.ResetState();
        attackTimer = 0f;
        localCoroutine = null;
    }
}
