using UnityEngine;

public class BowAttackExecutor : CombatActionExecutor
{
    private GameObject arrowPrefab;

    private float arrowSpeed = 15f;
    private float arrowHeightOffset = 1.2f;
    private float cooldownTime = 2f;
    private float damageDelay = 0.4f;
    private float attackAnimationTime = 1.0f;

    private float cooldownTimer;
    private float damageTimer;
    private float attackTimer;
    private bool arrowPending;

    private Vector3 targetPosition;

    public BowAttackExecutor(Character character, MoveCommandHandler movement, Animator animator, GameObject arrowPrefab)
        : base(character, movement, animator)
    {
        this.arrowPrefab = arrowPrefab;
    }

    public override void TickUpdate()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;

        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;

            if (damageTimer <= 0f && arrowPending)
            {
                FireArrow();
                arrowPending = false;
                cooldownTimer = cooldownTime;
            }
        }
    }

    public override void Execute(Command command)
    {
        // ❌ Prevent spamming: ignore input if attack in progress or on cooldown
        if (attackTimer > 0f || cooldownTimer > 0f)
            return;

        // Determine aim point
        targetPosition = GetAimPosition(command);

        // Face target
        FaceDirection(targetPosition);

        // Stop movement and trigger animation
        StopMovement();
        TriggerAttackAnimation();

        // Setup timers
        attackTimer = attackAnimationTime;
        damageTimer = damageDelay;
        arrowPending = true;
    }

    private void FireArrow()
    {
        if (arrowPrefab == null)
            return;

        Vector3 spawnPos = character.transform.position + Vector3.up * arrowHeightOffset + character.transform.forward * 0.5f;
        Vector3 direction = (targetPosition - spawnPos).normalized;

        var arrowObj = Object.Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        arrowObj.GetComponent<Arrow>().Initialize(character, direction, arrowSpeed, arrowHeightOffset);
    }

    private Vector3 GetAimPosition(Command command)
    {
        Camera cam = Camera.main;
        if (character.IsPlayer && cam != null)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, character.transform.position + Vector3.up * arrowHeightOffset);

            if (plane.Raycast(ray, out float distance))
                return ray.GetPoint(distance);
        }

        // Fallback for AI or no camera
        Vector3 fallback = command != null ? command.worldPoint : character.transform.position + character.transform.forward * 10f;
        fallback.y = character.transform.position.y + arrowHeightOffset;
        return fallback;
    }

    private void TriggerAttackAnimation()
    {
        if (animator == null) return;

        if (character.IsPlayer && AnimatorHasTrigger("BowAttack"))
            animator.SetTrigger("BowAttack");
        else if (AnimatorHasTrigger("Attack"))
            animator.SetTrigger("Attack");
    }

    public void CancelCurrentAttack()
    {
        arrowPending = false;
        attackTimer = 0f;
        damageTimer = 0f;
        ResetAnimatorTriggers("BowAttack", "Attack", "FistAttack");
    }

    protected override void ResetState() => CancelCurrentAttack();

    public override bool HasActiveTarget() => arrowPending;

    protected override float ApplyCooldown(float baseCooldown) => baseCooldown;
}
