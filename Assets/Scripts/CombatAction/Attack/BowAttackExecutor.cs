using UnityEngine;

public class BowAttackExecutor : CombatActionExecutor
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float arrowSpeed = 15f;
    [SerializeField] private float arrowHeightOffset = 1.2f;
    [SerializeField] private float arrowSpawnProgress = 0.5f;
    [SerializeField] private float defaultTimeToAttack = 1f;
    [SerializeField] private float attackAnimationTime = 1f;

    private float cooldownTimer;
    private float animationTimer;
    private bool isAttackLocked;
    private bool isAttacking;
    private bool hasSpawnedArrow;
    private Vector3 targetPosition;

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (!isAttacking)
            return;

        StopMovement();
        animationTimer -= Time.deltaTime;
        float progress = 1f - (animationTimer / attackAnimationTime);
        isAttackLocked = progress >= 0.3f && progress <= 0.6f;
        SetPerformingCombatAction(isAttackLocked);

        if (!hasSpawnedArrow && progress >= arrowSpawnProgress)
        {
            SpawnArrowAtPosition(targetPosition);
            hasSpawnedArrow = true;
            cooldownTimer = ApplyCooldown(defaultTimeToAttack);
        }

        if (animationTimer <= 0f)
        {
            isAttacking = false;
            isAttackLocked = false;
            hasSpawnedArrow = false;
            SetPerformingCombatAction(false);
        }
    }

    public override void Execute(Command command)
    {
        if (cooldownTimer > 0f || isAttacking) return;

        if (command.target != null)
            targetPosition = command.target.transform.position + Vector3.up * arrowHeightOffset;
        else
            targetPosition = transform.position + transform.forward * 10f + Vector3.up * arrowHeightOffset;

        StopMovement();
        RotateTowardsPoint(targetPosition);
        PrepareWeapon();
        PlayAttackAnimation();

        isAttacking = true;
        hasSpawnedArrow = false;
    }

    private void SpawnArrowAtPosition(Vector3 targetPos)
    {
        if (arrowPrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * arrowHeightOffset + transform.forward * 0.5f;
        GameObject arrowObject = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

        if (character.IsPlayer)
            arrowObject.layer = LayerMask.NameToLayer("PlayerProjectile");
        else
            arrowObject.layer = LayerMask.NameToLayer("EnemyProjectile");

        Arrow arrowScript = arrowObject.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            Vector3 flatTarget = new Vector3(targetPos.x, spawnPos.y, targetPos.z);
            Vector3 dir = (flatTarget - spawnPos).normalized;
            arrowScript.Initialize(character, dir, arrowSpeed, arrowHeightOffset);
        }
        else Destroy(arrowObject);
    }

    private void PrepareWeapon()
    {
        SetPerformingCombatAction(true);
        character.GetComponent<CharacterWeaponVisibilityController>()?.ResetLingerTimer();
    }

    private void PlayAttackAnimation()
    {
        animationTimer = attackAnimationTime;
        isAttackLocked = false;
        TriggerAttackAnimation();
    }

    private void TriggerAttackAnimation()
    {
        string trigger = null;
        if (character.IsPlayer)
        {
            InventoryItem weapon = character.GetComponent<PlayerInventory>()?.CurrentWeapon;
            WeaponType type = weapon != null ? weapon.itemData.weaponType : WeaponType.None;

            if (type == WeaponType.Bow && AnimatorHasTrigger("BowAttack")) trigger = "BowAttack";
            else if (AnimatorHasTrigger("Attack")) trigger = "Attack";
            else if (AnimatorHasTrigger("FistAttack")) trigger = "FistAttack";
        }
        else
        {
            AICombat aiCombat = character.GetComponent<AICombat>();
            if (aiCombat != null && aiCombat.WeaponType == AIWeaponType.Bow && AnimatorHasTrigger("BowAttack"))
                trigger = "BowAttack";
            else if (AnimatorHasTrigger("Attack")) trigger = "Attack";
        }

        if (!string.IsNullOrEmpty(trigger))
            animator.SetTrigger(trigger);
    }

    public override void ResetState()
    {
        isAttacking = false;
        animationTimer = 0f;
        isAttackLocked = false;
        hasSpawnedArrow = false;
        SetPerformingCombatAction(false);
        ResetAnimatorTriggers("BowAttack", "Attack", "FistAttack");
    }

    protected override float ApplyCooldown(float baseCooldown)
    {
        return baseCooldown / character.GetStatsValue(RegularStat.AttackSpeed).float_value;
    }
}
