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
    private bool isSpawningArrow;
    private Vector3 targetPosition;
    private bool isActive;

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (isActive)
        {
            if (animationTimer > 0f)
            {
                animationTimer -= Time.deltaTime;
                float progress = 1f - (animationTimer / attackAnimationTime);
                isAttackLocked = progress >= 0.3f && progress <= 0.6f;

                if (!isSpawningArrow && progress >= arrowSpawnProgress)
                {
                    SpawnArrowAtPosition(targetPosition);
                    isSpawningArrow = true;
                    cooldownTimer = ApplyCooldown(defaultTimeToAttack);
                }
            }
            else
            {
                isActive = false;
                isAttackLocked = false;
                isSpawningArrow = false;
            }

            SetPerformingCombatAction(isAttackLocked);
        }
    }

    public override void Execute(Command command)
    {
        if (!CanAttack()) return;

        if (command != null && command.target != null)
            targetPosition = command.target.transform.position + Vector3.up * arrowHeightOffset;
        else
            targetPosition = GetMouseWorldPosition();

        StopMovementAndRotate(targetPosition);
        PrepareWeapon();
        PlayAttackAnimation();

        isActive = true;
        isSpawningArrow = false;
    }

    public override void ResetState()
    {
        base.ResetState();
        animationTimer = 0f;
        isAttackLocked = false;
        isSpawningArrow = false;
        isActive = false;
        SetPerformingCombatAction(false);
        ResetAnimatorTriggers("BowAttack", "Attack", "FistAttack");
    }

    private bool CanAttack()
    {
        return cooldownTimer <= 0f && !isActive;
    }

    private void StopMovementAndRotate(Vector3 targetPos)
    {
        StopMovement();
        RotateTowardsPoint(targetPos);
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
        else
        {
            Destroy(arrowObject);
        }
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

    protected override float ApplyCooldown(float baseCooldown)
    {
        return baseCooldown / character.GetStatsValue(RegularStat.AttackSpeed).float_value;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position + Vector3.up * arrowHeightOffset);
        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);
        return transform.position + transform.forward * 10f + Vector3.up * arrowHeightOffset;
    }
}
