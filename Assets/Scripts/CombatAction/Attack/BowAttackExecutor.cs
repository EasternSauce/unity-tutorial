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
    private Quaternion lockedRotation;

    public bool IsBusyAttacking() => attackTimer > 0f;

    public BowAttackExecutor(Character character, MoveCommandHandler movement, Animator animator, GameObject arrowPrefab)
        : base(character, movement, animator)
    {
        this.arrowPrefab = arrowPrefab;
    }

    public override void Execute(Command command)
    {
        if (attackTimer > 0f || cooldownTimer > 0f)
            return;

        targetPosition = GetTargetPosition(command);
        FaceDirection(targetPosition);
        lockedRotation = character.transform.rotation;
        movement?.Stop();
        TriggerAttackAnimation();
        attackTimer = attackAnimationTime;
        damageTimer = damageDelay;
        arrowPending = true;
    }

    public override void TickUpdate()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
            movement?.Stop();
            character.transform.rotation = lockedRotation;
        }
        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f && arrowPending)
                FireArrow();
        }
    }

    private Vector3 GetTargetPosition(Command command)
    {
        if (character.IsPlayer) return GetPlayerAimPosition();
        if (command != null && command.target != null) return GetAIPosition(command.target);
        return GetFallbackPosition(command);
    }

    private Vector3 GetPlayerAimPosition()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            Plane plane = new Plane(Vector3.up, character.transform.position + Vector3.up * arrowHeightOffset);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float distance)) return ray.GetPoint(distance);
        }
        return GetFallbackPosition(null);
    }

    private Vector3 GetAIPosition(GameObject target)
    {
        Vector3 pos = target.transform.position;
        pos.y = character.transform.position.y + arrowHeightOffset;
        return pos;
    }

    private Vector3 GetFallbackPosition(Command command)
    {
        Vector3 pos = (command != null && command.worldPoint != Vector3.zero)
            ? command.worldPoint
            : character.transform.position + character.transform.forward * 10f;
        pos.y = character.transform.position.y + arrowHeightOffset;
        return pos;
    }

    private void FireArrow()
    {
        if (arrowPrefab == null) return;
        Vector3 spawnPos = character.transform.position + Vector3.up * arrowHeightOffset + character.transform.forward * 0.5f;
        Vector3 direction = (targetPosition - spawnPos).normalized;
        var arrowObj = Object.Instantiate(arrowPrefab, spawnPos, Quaternion.LookRotation(direction));
        arrowObj.GetComponent<Arrow>().Initialize(character, direction, arrowSpeed, arrowHeightOffset);
        arrowPending = false;
        cooldownTimer = cooldownTime;
    }

    private void TriggerAttackAnimation()
    {
        if (animator == null) return;
        if (AnimatorHasTrigger("BowAttack")) animator.SetTrigger("BowAttack");
    }

    public void CancelCurrentAttack()
    {
        arrowPending = false;
        attackTimer = 0f;
        damageTimer = 0f;
        ResetAnimatorTriggers("BowAttack", "Attack");
    }

    protected override void ResetState() => CancelCurrentAttack();
    public override bool HasActiveTarget() => arrowPending;
    protected override float ApplyCooldown(float baseCooldown) => baseCooldown;
}
