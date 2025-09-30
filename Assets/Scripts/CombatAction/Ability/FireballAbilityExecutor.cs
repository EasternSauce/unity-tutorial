using UnityEngine;

/*
FireballAbilityExecutor

Functional overview:
- Fireball is a ranged magical attack that spawns a fireball projectile.
- Unlike melee, there is no walk-up phase. The caster begins casting immediately.
- For the player:
  - Fireball is aimed exactly like bow attacks: cast a ray from the camera through the cursor.
  - A point along that ray is chosen, and the projectile is fired so that it travels through that point.
  - This ensures the fireball lines up perfectly with the cursor in 3D space.
- For AI:
  - Fireball is cast toward the target’s current position.
- Attack phases:
  1. Casting begins when command is issued (if off cooldown).
  2. Animation plays, locking the character briefly (attack lock phase).
  3. When progress reaches spawnProgress, the fireball projectile is spawned and travels forward.
  4. Cooldown is applied immediately when the fireball is spawned.
  5. Casting ends when the animation completes or is interrupted.
- Cancelation rules:
  - Player or AI movement cancels the attack before the projectile spawns.
  - If cancelled early, no projectile spawns and no cooldown is applied.
  - If cancelled after spawn, the projectile still exists and cooldown is active.
- Differences from melee and bow:
  - Fireball is a one-off ability, not auto-repeat.
  - Fireball does not track a target; it simply travels along its launch vector.
  - Unlike bow, fireball uses a spell-cast animation trigger.
*/
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

        if (character.IsPlayer)
        {
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                targetPosition = ray.origin + ray.direction * 50f; // far point along ray
            }
        }
        else
        {
            targetPosition = (command.target != null)
                ? command.target.transform.position + Vector3.up * heightOffset
                : character.transform.position + character.transform.forward * 10f + Vector3.up * heightOffset;
        }

        StopMovement();
        RotateTowardsPoint(targetPosition);

        animationTimer = attackTime;
        isAttackLocked = false;
        isCasting = true;

        if (AnimatorHasTrigger("SpellCast"))
            animator.SetTrigger("SpellCast");
    }

    private void SpawnFireball(Vector3 target)
    {
        if (fireballPrefab == null) return;

        Vector3 spawn = character.transform.position + Vector3.up * heightOffset;
        Vector3 dir = (target - spawn).normalized;

        GameObject proj = Object.Instantiate(fireballPrefab, spawn, Quaternion.identity);
        Fireball f = proj.GetComponent<Fireball>();
        if (f != null)
            f.Initialize(character, dir, speed, heightOffset);
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
