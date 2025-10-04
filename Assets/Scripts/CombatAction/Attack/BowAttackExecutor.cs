using UnityEngine;

/*
BowAttackExecutor.cs

Purpose:
- Executes bow combat actions for characters (player or AI).
- Handles attack animation, arrow spawning, and cooldown management.

Functional Requirements / Expected Behavior:

1. Phases of bow attack:
   - AttackStart: Immediately starts attack animation and prepares to release an arrow.
   - ArrowSpawn: At a defined point in the animation, a single arrow is spawned and fired in a direction.
   - AttackEnd: Attack continues until animation completes or is cancelled.

2. Targeting:
   - Player: arrow is fired toward the mouse cursor, projected onto a horizontal plane at bow height.
   - AI: arrow is fired toward the target's current position, or forward if no target.
   - The arrow hits the first object in its path; no automatic homing.

3. Arrow Physics:
   - Arrows travel in a straight line with constant velocity.
   - Gravity is not applied; Y remains constant relative to the initial direction.
   - Arrow rotation follows velocity.

4. Cooldown:
   - Applied only at arrow spawn.
   - Cancelling before spawn means no cooldown.

5. Cancel Behavior:
   - Attack can be cancelled anytime.
   - Cancel before arrow spawn = no cooldown, no arrow.
   - Cancel after arrow spawn = stops animation, arrow remains.

6. Mouse Aiming:
   - For the player, aiming is calculated using `GetMouseWorldPosition`.
   - This method casts a ray from the camera through the mouse position.
   - The ray is intersected with a horizontal plane placed at the character’s bow height
     (character.transform.position + Vector3.up * arrowHeightOffset).
   - This projects the cursor onto an infinite flat plane at bow height so aiming is consistent
     regardless of terrain height or objects between camera and ground.
   - If the ray somehow misses the plane, the fallback is a point forward from the character.

Notes:
- Animator triggers differ for player and AI.
- Public methods should not be removed unless unused externally.
*/

public class BowAttackExecutor : CombatActionExecutor
{
    private GameObject arrowPrefab;
    private float arrowSpeed;
    private float arrowHeightOffset;
    private float arrowSpawnProgress;
    private float attackAnimationTime;
    private float defaultCooldown;

    private float cooldownTimer;
    private float animationTimer;
    private bool isAttackLocked;
    private bool hasSpawnedArrow;
    private bool isAttacking;
    private Vector3 targetPosition;

    public BowAttackExecutor(
        Character character,
        MoveCommandHandler movement,
        Animator animator,
        GameObject prefab,
        float arrowSpeed = 15f,
        float arrowHeightOffset = 1.2f,
        float arrowSpawnProgress = 0.5f,
        float attackAnimationTime = 1f,
        float defaultCooldown = 1f
    ) : base(character, movement, animator)
    {
        this.arrowPrefab = prefab;
        this.arrowSpeed = arrowSpeed;
        this.arrowHeightOffset = arrowHeightOffset;
        this.arrowSpawnProgress = arrowSpawnProgress;
        this.attackAnimationTime = attackAnimationTime;
        this.defaultCooldown = defaultCooldown;

        cooldownTimer = 0f;
        animationTimer = 0f;
        isAttackLocked = false;
        hasSpawnedArrow = false;
        isAttacking = false;
    }

    public override void TickUpdate()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (!isAttacking) return;

        if (animationTimer > 0f)
        {
            animationTimer -= Time.deltaTime;
            float progress = 1f - (animationTimer / attackAnimationTime);

            isAttackLocked = progress >= 0.3f && progress <= 0.6f;

            if (!hasSpawnedArrow && progress >= arrowSpawnProgress)
            {
                SpawnArrowAtPosition(targetPosition);
                hasSpawnedArrow = true;
                cooldownTimer = ApplyCooldown(defaultCooldown);
            }
        }
        else
        {
            isAttacking = false;
            isAttackLocked = false;
            hasSpawnedArrow = false;
        }

        SetPerformingCombatAction(isAttackLocked);
    }

    public override void Execute(Command command)
    {
        if (!CanAttack()) return;

        if (character.IsPlayer)
        {
            targetPosition = GetMouseWorldPosition();
        }
        else if (command != null && command.target != null)
        {
            targetPosition = command.target.transform.position + Vector3.up * arrowHeightOffset;
        }
        else
        {
            targetPosition = character.transform.position + character.transform.forward * 10f + Vector3.up * arrowHeightOffset;
        }

        StopMovement();
        RotateTowardsPoint(targetPosition);
        PrepareWeapon();
        PlayAttackAnimation();

        isAttacking = true;
        hasSpawnedArrow = false;
    }

    protected override void ResetState()
    {
        animationTimer = 0f;
        isAttackLocked = false;
        hasSpawnedArrow = false;
        isAttacking = false;
        SetPerformingCombatAction(false);
        ResetAnimatorTriggers("BowAttack", "Attack", "FistAttack");
    }

    private bool CanAttack()
    {
        return cooldownTimer <= 0f && !isAttacking;
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

        Vector3 spawnPos = character.transform.position + Vector3.up * arrowHeightOffset + character.transform.forward * 0.5f;
        GameObject arrowObject = Object.Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

        if (character.IsPlayer)
            arrowObject.layer = LayerMask.NameToLayer("PlayerProjectile");
        else
            arrowObject.layer = LayerMask.NameToLayer("EnemyProjectile");

        Arrow arrowScript = arrowObject.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            Vector3 dir = (targetPos - spawnPos).normalized;
            arrowScript.Initialize(character, dir, arrowSpeed, arrowHeightOffset);
        }
        else
        {
            Object.Destroy(arrowObject);
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
        Camera cam = Camera.main;
        if (cam == null)
        {
            return character.transform.position + character.transform.forward * 10f + Vector3.up * arrowHeightOffset;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, character.transform.position + Vector3.up * arrowHeightOffset);

        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return character.transform.position + character.transform.forward * 10f + Vector3.up * arrowHeightOffset;
    }
}
