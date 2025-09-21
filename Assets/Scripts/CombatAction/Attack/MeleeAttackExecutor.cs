using System.Collections;
using UnityEngine;

public class MeleeAttackExecutor : CombatActionExecutor
{
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private float defaultTimeToAttack = 1f;
    [SerializeField] private float attackAnimationTime = 1f;

    private float attackTimer;
    private GameObject currentTarget;
    private AttackPhase currentPhase = AttackPhase.None;
    private float phaseTimer;
    private bool hasDealtDamage;

    private enum AttackPhase { None, Windup, Damage, Recovery }

    private void Update()
    {
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
        if (phaseTimer > 0f) phaseTimer -= Time.deltaTime;
        SetPerformingCombatAction(currentPhase != AttackPhase.None);
    }

    public void HandleMeleeAttack(Command command)
    {
        if (command.target == null || character == null || character.IsDead) return;
        if (command.target.GetComponent<Character>()?.IsDead == true) return;

        if (currentTarget != command.target) CancelCurrentAttack();
        else if (combatActionCoroutine != null) return;

        currentTarget = command.target;
        combatActionCoroutine = StartCoroutine(MeleeAttackRoutine(command));
    }

    private IEnumerator MeleeAttackRoutine(Command command)
    {
        while (currentTarget != null && character != null && !character.IsDead)
        {
            Character targetCharacter = currentTarget.GetComponent<Character>();
            if (targetCharacter != null && targetCharacter.IsDead)
            {
                currentTarget = null;
                break;
            }

            Transform targetTransform = currentTarget.transform;
            float distance = Vector3.Distance(transform.position, targetTransform.position);
            float range = attackRange;

            InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
            if (weapon == null || weapon.itemData.weaponType == WeaponType.None) range = 1.5f;

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

                if (characterMovement.Agent != null && characterMovement.Agent.enabled && characterMovement.Agent.isOnNavMesh)
                {
                    characterMovement.Agent.stoppingDistance = 0f;
                    characterMovement.Agent.isStopped = false;
                    characterMovement.SetDestination(destination);
                }
            }
            yield return null;
        }

        if (characterMovement.Agent != null && characterMovement.Agent.enabled && characterMovement.Agent.isOnNavMesh)
            characterMovement.Agent.stoppingDistance = characterMovement.DefaultStoppingDistance;

        StopAndClearCoroutine(ref combatActionCoroutine);
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
        if (hasDealtDamage || currentTarget == null || character == null || character.IsDead) return;

        if (currentTarget.TryGetComponent<IDamageable>(out var damageable))
        {
            if (!(damageable is Character c) || !c.IsDead)
            {
                damageable.TakeDamage(character.GetDamage());

                if (damageable is Character deadChar && deadChar.IsDead)
                {
                    currentTarget = null;
                    StopAndClearCoroutine(ref combatActionCoroutine);
                    return;
                }
            }
        }

        hasDealtDamage = true;
        attackTimer = ApplyCooldown(defaultTimeToAttack);
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
        if (type == WeaponType.OneHandedAxe && AnimatorHasTrigger("OneHandedMeleeAttack")) trigger = "OneHandedMeleeAttack";
        else if (type == WeaponType.TwoHandedAxe && AnimatorHasTrigger("TwoHandedMeleeAttack")) trigger = "TwoHandedMeleeAttack";
        else if (AnimatorHasTrigger("Attack")) trigger = "Attack";
        else if (AnimatorHasTrigger("FistAttack")) trigger = "FistAttack";

        if (!string.IsNullOrEmpty(trigger))
        {
            animator.Update(0f);
            animator.SetTrigger(trigger);
        }
    }

    public void CancelCurrentAttack()
    {
        StopAndClearCoroutine(ref combatActionCoroutine);
        currentTarget = null;
        currentPhase = AttackPhase.None;
    }

    public override void ResetState()
    {
        base.ResetState();
        CancelCurrentAttack();
        hasDealtDamage = false;
        SetPerformingCombatAction(false);
        ResetAnimatorTriggers("Attack", "FistAttack", "OneHandedMeleeAttack", "TwoHandedMeleeAttack");
    }

    override protected float ApplyCooldown(float baseCooldown)
    {
        return baseCooldown / character.GetStatsValue(RegularStat.AttackSpeed).float_value;
    }
}
