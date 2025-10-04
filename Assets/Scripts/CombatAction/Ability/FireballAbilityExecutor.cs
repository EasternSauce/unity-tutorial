using UnityEngine;

/*
FireballAbilityExecutor

- Fireball is a ranged magical attack that travels at a fixed height.
- Player aims using mouse projected onto horizontal plane at character height.
- AI aims at target’s position, or forward if no target.
- Casting phases and cooldown match bow attack behavior.
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
    private bool hasSpawnedFireball;
    private Vector3 targetPosition;

    public FireballAbilityExecutor(
        Character character,
        MoveCommandHandler movement,
        Animator animator,
        GameObject prefab,
        float speed = 15f,
        float heightOffset = 1.2f,
        float attackTime = 1f,
        float spawnProgress = 0.95f,
        float defaultCooldown = 1f
    ) : base(character, movement, animator)
    {
        fireballPrefab = prefab;
        this.speed = speed;
        this.heightOffset = heightOffset;
        this.attackTime = attackTime;
        this.spawnProgress = spawnProgress;
        this.defaultCooldown = defaultCooldown;
        cooldownTimer = 0f;
        animationTimer = 0f;
        isCasting = false;
        isAttackLocked = false;
        hasSpawnedFireball = false;
    }

    public override void TickUpdate()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (!isCasting) return;

        StopMovement();

        if (animationTimer > 0f)
        {
            animationTimer -= Time.deltaTime;
            float progress = 1f - (animationTimer / attackTime);

            isAttackLocked = progress >= 0.3f && progress <= 0.6f;

            if (!hasSpawnedFireball && progress >= spawnProgress)
            {
                SpawnFireball(targetPosition);
                hasSpawnedFireball = true;
                cooldownTimer = ApplyCooldown(defaultCooldown);
            }
        }
        else
        {
            isCasting = false;
            isAttackLocked = false;
            hasSpawnedFireball = false;
        }
    }

    public override void Execute(Command command)
    {
        if (cooldownTimer > 0f || isCasting) return;

        // Determine target position
        if (character.IsPlayer)
            targetPosition = GetMouseWorldPosition();
        else
            targetPosition = (command != null && command.target != null)
                ? command.target.transform.position + Vector3.up * heightOffset
                : character.transform.position + character.transform.forward * 10f + Vector3.up * heightOffset;

        StopMovement();

        animationTimer = attackTime;
        isCasting = true;
        hasSpawnedFireball = false;
        isAttackLocked = false;

        if (AnimatorHasTrigger("SpellCast"))
            animator.SetTrigger("SpellCast");
    }

    private void SpawnFireball(Vector3 target)
    {
        if (fireballPrefab == null) return;

        Vector3 spawnPos = character.transform.position + Vector3.up * heightOffset;
        Vector3 dir = (target - spawnPos).normalized;
        dir.y = 0f; // enforce horizontal flight
        dir.Normalize();

        GameObject proj = Object.Instantiate(fireballPrefab, spawnPos, Quaternion.LookRotation(dir));
        Fireball fireball = proj.GetComponent<Fireball>();
        if (fireball != null)
            fireball.Initialize(character, dir, speed, heightOffset);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return character.transform.position + character.transform.forward * 10f + Vector3.up * heightOffset;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, character.transform.position + Vector3.up * heightOffset);

        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return character.transform.position + character.transform.forward * 10f + Vector3.up * heightOffset;
    }

    protected override void ResetState()
    {
        isCasting = false;
        animationTimer = 0f;
        isAttackLocked = false;
        hasSpawnedFireball = false;
        ResetAnimatorTriggers("SpellCast");
    }

    protected override float ApplyCooldown(float baseCooldown)
    {
        return baseCooldown / character.GetStatsValue(RegularStat.AttackSpeed).float_value;
    }
}
