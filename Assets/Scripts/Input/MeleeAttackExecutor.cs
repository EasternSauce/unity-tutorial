using System.Collections;
using UnityEngine;

public class MeleeAttackExecutor : AttackExecutor
{
    [SerializeField] float attackRange = 2.5f;
    [SerializeField] float defaultTimeToAttack = 1f;
    [SerializeField] float attackAnimationTime = 1f;

    private float attackTimer;
    private Coroutine localCoroutine;

    private CanMoveState canMoveState;
    private GameObject currentTarget;

    private enum AttackPhase
    {
        None,
        Windup,
        Damage,
        Recovery
    }

    private AttackPhase currentPhase = AttackPhase.None;
    private float phaseTimer;
    private bool hasDealtDamage = false;

    protected override void Awake()
    {
        base.Awake();
        canMoveState = GetComponent<CanMoveState>();
    }

    private void Update()
    {
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
        if (phaseTimer > 0f) phaseTimer -= Time.deltaTime;

        if (canMoveState != null)
            canMoveState.isAttacking = currentPhase != AttackPhase.None;
    }

    public void HandleMeleeAttack(Command command)
    {
        if (command.target == null) return;

        if (currentTarget != command.target)
        {
            CancelCurrentAttack();
        }
        else if (localCoroutine != null)
        {
            return;
        }

        currentTarget = command.target;
        localCoroutine = StartCoroutine(MeleeAttackRoutine(command));
    }

    private IEnumerator MeleeAttackRoutine(Command command)
    {
        while (currentTarget != null)
        {
            // ✅ Step 1: stop attacking if target is dead
            IDamageable damageable = currentTarget.GetComponent<IDamageable>();
            if (damageable == null || (damageable is Character c && c.IsDead))
            {
                CancelCurrentAttack();
                yield break;
            }

            Transform targetTransform = currentTarget.transform;
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            float range = attackRange;

            InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
            if (weapon == null || weapon.itemData.weaponType == WeaponType.None)
                range = 1.5f;

            if (distance <= range + 0.1f)
            {
                StopMovement();
                RotateTowardsTarget(targetTransform, true);

                if (attackTimer <= 0f && currentPhase == AttackPhase.None)
                {
                    StartAttackPhase();
                    yield return new WaitForSeconds(attackAnimationTime * 0.4f);
                    ExecuteDamageOnTarget();
                    yield return new WaitForSeconds(attackAnimationTime * 0.6f);
                    EndAttackPhase();
                }
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


    private void StartAttackPhase()
    {
        hasDealtDamage = false;
        currentPhase = AttackPhase.Windup;
        phaseTimer = attackAnimationTime;
        TriggerAttackAnimation();
    }

    private void ExecuteDamageOnTarget()
    {
        if (hasDealtDamage || currentTarget == null)
            return;

        IDamageable target = currentTarget.GetComponent<IDamageable>();
        if (target != null)
            target.TakeDamage(character.GetDamage());

        hasDealtDamage = true;
        ApplyCooldown();
        currentPhase = AttackPhase.Damage;
    }

    private void EndAttackPhase()
    {
        currentPhase = AttackPhase.None;
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
        else if (AnimatorHasTrigger("Attack"))
            trigger = "Attack";
        else if (AnimatorHasTrigger("FistAttack"))
            trigger = "FistAttack";

        if (!string.IsNullOrEmpty(trigger))
        {
            animator.Update(0f);
            animator.SetTrigger(trigger);
        }
    }

    private bool AnimatorHasTrigger(string name)
    {
        foreach (var p in animator.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger && p.name == name)
                return true;
        return false;
    }

    public void CancelCurrentAttack()
    {
        if (localCoroutine != null)
        {
            StopCoroutine(localCoroutine);
            localCoroutine = null;
        }
        currentTarget = null;
        currentPhase = AttackPhase.None;
    }

    public override void ResetState()
    {
        base.ResetState();
        CancelCurrentAttack();
        hasDealtDamage = false;
        if (canMoveState != null) canMoveState.isAttacking = false;

        if (AnimatorHasTrigger("Attack")) animator.ResetTrigger("Attack");
        if (AnimatorHasTrigger("FistAttack")) animator.ResetTrigger("FistAttack");
        if (AnimatorHasTrigger("OneHandedMeleeAttack")) animator.ResetTrigger("OneHandedMeleeAttack");
        if (AnimatorHasTrigger("TwoHandedMeleeAttack")) animator.ResetTrigger("TwoHandedMeleeAttack");
    }

    private void ApplyCooldown()
    {
        attackTimer = defaultTimeToAttack / character.GetStatsValue(Statistic.AttackSpeed).float_value;
    }
}
