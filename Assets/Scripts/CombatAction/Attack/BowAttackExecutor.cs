using UnityEngine;

public class BowAttackExecutor : CombatActionExecutor
{
    private GameObject arrowPrefab;
    private float arrowSpeed = 15f;
    private float arrowHeightOffset = 1.2f;
    private float cooldownTime = 1f;
    private float cooldownTimer;

    public BowAttackExecutor(Character character, MoveCommandHandler movement, Animator animator, GameObject arrowPrefab)
        : base(character, movement, animator)
    {
        this.arrowPrefab = arrowPrefab;
    }

    public override void TickUpdate()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public override void Execute(Command command)
    {
        if (cooldownTimer > 0f)
            return;

        // 1️⃣ Get aim point (flat plane at bow height)
        Vector3 targetPosition = GetFlatAimPoint(command);

        // 2️⃣ Stop and animate
        StopMovement();
        TriggerAttackAnimation();

        // 3️⃣ Spawn arrow
        Vector3 spawnPos = character.transform.position + Vector3.up * arrowHeightOffset + character.transform.forward * 0.5f;
        Vector3 direction = (targetPosition - spawnPos).normalized;

        var arrowObj = Object.Instantiate(arrowPrefab, spawnPos, Quaternion.identity);
        arrowObj.GetComponent<Arrow>().Initialize(character, direction, arrowSpeed, arrowHeightOffset);

        cooldownTimer = cooldownTime;
    }

    private Vector3 GetFlatAimPoint(Command command)
    {
        if (character.IsPlayer)
        {
            Camera cam = Camera.main;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, character.transform.position + Vector3.up * arrowHeightOffset);

            if (plane.Raycast(ray, out float distance))
                return ray.GetPoint(distance);
        }

        Vector3 basePos = command != null ? command.worldPoint : character.transform.position + character.transform.forward * 10f;
        basePos.y = character.transform.position.y + arrowHeightOffset;
        return basePos;
    }

    private void TriggerAttackAnimation()
    {
        if (animator == null) return;

        if (character.IsPlayer && AnimatorHasTrigger("BowAttack"))
            animator.SetTrigger("BowAttack");
        else if (AnimatorHasTrigger("Attack"))
            animator.SetTrigger("Attack");
    }

    protected override void ResetState()
    {
        cooldownTimer = 0f;
        ResetAnimatorTriggers("BowAttack", "Attack", "FistAttack");
    }

    protected override float ApplyCooldown(float baseCooldown) => baseCooldown;
}
