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
        // Prevent spamming: ignore if attack in progress or cooldown
        if (attackTimer > 0f || cooldownTimer > 0f)
            return;

        // Determine aim point
        targetPosition = GetTargetPosition(command);

        // Face the target
        FaceDirection(targetPosition);

        // Stop movement and play animation
        StopMovement();
        TriggerAttackAnimation();

        // Setup timers
        attackTimer = attackAnimationTime;
        damageTimer = damageDelay;
        arrowPending = true;
    }

    private void FireArrow()
    {
        if (arrowPrefab == null) return;

        Vector3 spawnPos = character.transform.position + Vector3.up * arrowHeightOffset + character.transform.forward * 0.5f;
        Vector3 direction = (targetPosition - spawnPos).normalized;

        var arrowObj = Object.Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        arrowObj.GetComponent<Arrow>().Initialize(character, direction, arrowSpeed, arrowHeightOffset);
    }

    private Vector3 GetTargetPosition(Command command)
    {
        // Player aiming
        if (character.IsPlayer)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Plane plane = new Plane(Vector3.up, character.transform.position + Vector3.up * arrowHeightOffset);

                if (plane.Raycast(ray, out float distance))
                    return ray.GetPoint(distance);
            }
        }

        // AI aiming
        if (command != null)
        {
            if (command.target != null)
            {
                Vector3 pos = command.target.transform.position;
                pos.y = character.transform.position.y + arrowHeightOffset;
                return pos;
            }
            else if (command.worldPoint != Vector3.zero)
            {
                Vector3 pos = command.worldPoint;
                pos.y = character.transform.position.y + arrowHeightOffset;
                return pos;
            }
        }

        // Fallback
        Vector3 fallback = character.transform.position + character.transform.forward * 10f;
        fallback.y = character.transform.position.y + arrowHeightOffset;
        return fallback;
    }

    private void TriggerAttackAnimation()
    {
        if (animator == null) return;

        // Player animation
        if (character.IsPlayer && AnimatorHasTrigger("BowAttack"))
            animator.SetTrigger("BowAttack");
        // AI animation
        else if (!character.IsPlayer && AnimatorHasTrigger("BowAttack"))
            animator.SetTrigger("BowAttack");
        // fallback
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
