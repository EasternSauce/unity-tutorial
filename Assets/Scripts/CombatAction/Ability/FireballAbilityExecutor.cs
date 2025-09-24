using UnityEngine;

public class FireballAbilityExecutor : CombatActionExecutor
{
    private GameObject fireballPrefab;
    private float speed;
    private float heightOffset;
    private float attackTime;
    private float spawnProgress;
    private float defaultCooldown;

    private float cooldownTimer;
    private float animationTimer;
    private bool isCasting;
    private bool isAttackLocked;
    private Vector3 targetPosition;

    public FireballAbilityExecutor(Character character, MoveCommandHandler movement, Animator animator, GameObject prefab, float speed = 15f, float heightOffset = 1.2f, float attackTime = 1f, float spawnProgress = 0.95f, float defaultCooldown = 1f)
        : base(character, movement, animator)
    {
        fireballPrefab = prefab;
        this.speed = speed;
        this.heightOffset = heightOffset;
        this.attackTime = attackTime;
        this.spawnProgress = spawnProgress;
        this.defaultCooldown = defaultCooldown;
    }

    public override void TickUpdate()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (!isCasting) return;
        StopMovement();
        animationTimer -= Time.deltaTime;
        float progress = 1f - animationTimer / attackTime;
        isAttackLocked = progress >= 0.3f && progress <= 0.6f;
        SetPerformingCombatAction(isAttackLocked);

        if (progress >= spawnProgress)
        {
            SpawnFireball(targetPosition);
            cooldownTimer = ApplyCooldown(defaultCooldown);
            isCasting = false;
            isAttackLocked = false;
            SetPerformingCombatAction(false);
        }

        if (animationTimer <= 0f)
        {
            isCasting = false;
            isAttackLocked = false;
            SetPerformingCombatAction(false);
        }
    }

    public override void Execute(Command command)
    {
        if (cooldownTimer > 0f || isCasting) return;
        targetPosition = (command.target != null) ? command.target.transform.position + Vector3.up * heightOffset : character.transform.position + character.transform.forward * 10f + Vector3.up * heightOffset;
        StopMovement();
        RotateTowardsPoint(targetPosition);
        animationTimer = attackTime;
        isAttackLocked = false;
        isCasting = true;
        if (AnimatorHasTrigger("SpellCast")) animator.SetTrigger("SpellCast");
    }

    private void SpawnFireball(Vector3 target)
    {
        if (fireballPrefab == null) return;
        Vector3 spawn = character.transform.position + Vector3.up * heightOffset;
        Vector3 flatTarget = new Vector3(target.x, spawn.y, target.z);
        Vector3 dir = (flatTarget - spawn).normalized;
        GameObject proj = Object.Instantiate(fireballPrefab, spawn, Quaternion.identity);
        Fireball f = proj.GetComponent<Fireball>();
        if (f != null) f.Initialize(character, dir, speed, heightOffset);
    }

    public override void ResetState()
    {
        isCasting = false;
        animationTimer = 0f;
        isAttackLocked = false;
        SetPerformingCombatAction(false);
        ResetAnimatorTriggers("SpellCast");
    }

    protected override float ApplyCooldown(float baseCooldown) => baseCooldown;
}