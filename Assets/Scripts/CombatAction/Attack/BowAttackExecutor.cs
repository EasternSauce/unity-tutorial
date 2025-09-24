using UnityEngine;

public class BowAttackExecutor : CombatActionExecutor
{
    private GameObject arrowPrefab;
    private float speed;
    private float heightOffset;
    private float attackTime;
    private float spawnProgress;
    private float defaultCooldown;

    private float cooldownTimer;
    private float animationTimer;
    private bool isAttackLocked;
    private bool isAttacking;
    private bool hasSpawnedArrow;
    private Vector3 targetPosition;

    public BowAttackExecutor(Character character, MoveCommandHandler movement, Animator animator, GameObject prefab, float speed = 15f, float heightOffset = 1.2f, float attackTime = 1f, float spawnProgress = 0.5f, float defaultCooldown = 1f)
        : base(character, movement, animator)
    {
        arrowPrefab = prefab;
        this.speed = speed;
        this.heightOffset = heightOffset;
        this.attackTime = attackTime;
        this.spawnProgress = spawnProgress;
        this.defaultCooldown = defaultCooldown;
    }

    public override void TickUpdate()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (!isAttacking) return;
        StopMovement();
        animationTimer -= Time.deltaTime;
        float progress = 1f - animationTimer / attackTime;
        isAttackLocked = progress >= 0.3f && progress <= 0.6f;
        SetPerformingCombatAction(isAttackLocked);

        if (!hasSpawnedArrow && progress >= spawnProgress)
        {
            SpawnArrow(targetPosition);
            hasSpawnedArrow = true;
            cooldownTimer = ApplyCooldown(defaultCooldown);
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
        targetPosition = (command.target != null) ? command.target.transform.position + Vector3.up * heightOffset : character.transform.position + character.transform.forward * 10f + Vector3.up * heightOffset;
        StopMovement();
        RotateTowardsPoint(targetPosition);
        PrepareWeapon();
        animationTimer = attackTime;
        isAttackLocked = false;
        isAttacking = true;
        hasSpawnedArrow = false;
        TriggerAttackAnimation();
    }

    private void SpawnArrow(Vector3 target)
    {
        if (arrowPrefab == null) return;
        Vector3 spawn = character.transform.position + Vector3.up * heightOffset + character.transform.forward * 0.5f;
        GameObject arrowObj = Object.Instantiate(arrowPrefab, spawn, Quaternion.identity);
        if (character.IsPlayer) arrowObj.layer = LayerMask.NameToLayer("PlayerProjectile");
        else arrowObj.layer = LayerMask.NameToLayer("EnemyProjectile");
        Arrow a = arrowObj.GetComponent<Arrow>();
        if (a != null)
        {
            Vector3 flatTarget = new Vector3(target.x, spawn.y, target.z);
            Vector3 dir = (flatTarget - spawn).normalized;
            a.Initialize(character, dir, speed, heightOffset);
        }
        else Object.Destroy(arrowObj);
    }

    private void PrepareWeapon()
    {
        SetPerformingCombatAction(true);
        character.GetComponent<CharacterWeaponVisibilityController>()?.ResetLingerTimer();
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
            AICombat ai = character.GetComponent<AICombat>();
            if (ai != null && ai.WeaponType == AIWeaponType.Bow && AnimatorHasTrigger("BowAttack")) trigger = "BowAttack";
            else if (AnimatorHasTrigger("Attack")) trigger = "Attack";
        }
        if (!string.IsNullOrEmpty(trigger)) animator.SetTrigger(trigger);
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