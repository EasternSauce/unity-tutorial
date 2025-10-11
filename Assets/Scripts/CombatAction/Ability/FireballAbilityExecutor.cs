using UnityEngine;

public class FireballAbilityExecutor : CombatActionExecutor
{
    [SerializeField] private float fireballSpeed = 15f;
    [SerializeField] private float heightOffset = 1.2f;
    [SerializeField] private float cooldownTime = 3.0f;
    [SerializeField] private float damageDelay = 1.2f;
    [SerializeField] private float attackAnimationTime = 2.0f;

    private float cooldownTimer;
    private float damageTimer;
    private float attackTimer;
    private bool fireballPending;

    private GameObject currentTarget;
    private Vector3 targetPosition;
    private Quaternion lockedRotation;

    private GameObject fireballPrefab;

    public bool IsBusyCasting() => attackTimer > 0f;

    public FireballAbilityExecutor(Character character, MoveCommandHandler movement, Animator animator, GameObject fireballPrefab)
        : base(character, movement, animator)
    {
        this.fireballPrefab = fireballPrefab;
    }

    public override void Execute(Command command)
    {
        if (attackTimer > 0f || cooldownTimer > 0f || command == null)
            return;

        currentTarget = command.target;
        targetPosition = DetermineTargetPosition(command);
        FaceDirection(targetPosition);
        lockedRotation = character.transform.rotation;
        movement?.Stop();
        TriggerCastAnimation();

        attackTimer = attackAnimationTime;
        damageTimer = damageDelay;
        fireballPending = true;
    }

    public override void TickUpdate()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
            movement?.Stop();
            character.transform.rotation = lockedRotation;
        }

        if (damageTimer > 0f)
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0f && fireballPending)
                CastFireball();
        }
    }

    private Vector3 DetermineTargetPosition(Command command)
    {
        if (character.IsPlayer)
            return GetPlayerAimPosition();

        if (command.target != null)
            return GetAIPosition(command.target);

        return GetFallbackPosition(command);
    }

    private Vector3 GetPlayerAimPosition()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            Plane plane = new Plane(Vector3.up, character.transform.position + Vector3.up * heightOffset);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float distance))
                return ray.GetPoint(distance);
        }
        return GetFallbackPosition(null);
    }

    private Vector3 GetAIPosition(GameObject target)
    {
        Vector3 pos = target.transform.position;
        pos.y = character.transform.position.y + heightOffset;
        return pos;
    }

    private Vector3 GetFallbackPosition(Command command)
    {
        Vector3 pos = (command != null && command.worldPoint != Vector3.zero)
            ? command.worldPoint
            : character.transform.position + character.transform.forward * 10f;
        pos.y = character.transform.position.y + heightOffset;
        return pos;
    }

    private void CastFireball()
    {
        if (fireballPrefab == null) return;

        Vector3 spawnPos = character.transform.position + Vector3.up * heightOffset + character.transform.forward * 0.5f;
        Vector3 direction = (targetPosition - spawnPos).normalized;
        direction.y = 0f;
        direction.Normalize();

        var fireballObj = Object.Instantiate(fireballPrefab, spawnPos, Quaternion.LookRotation(direction));
        fireballObj.GetComponent<Fireball>().Initialize(character, direction, fireballSpeed, heightOffset);

        fireballPending = false;
        cooldownTimer = cooldownTime;
    }

    private void TriggerCastAnimation()
    {
        if (animator == null) return;
        if (AnimatorHasTrigger("SpellCast"))
            animator.SetTrigger("SpellCast");
    }

    public void CancelCurrentCast()
    {
        fireballPending = false;
        attackTimer = 0f;
        damageTimer = 0f;
        ResetAnimatorTriggers("SpellCast");
    }

    protected override void ResetState() => CancelCurrentCast();

    public override bool HasActiveTarget() => fireballPending;

    protected override float ApplyCooldown(float baseCooldown) => baseCooldown;
}
