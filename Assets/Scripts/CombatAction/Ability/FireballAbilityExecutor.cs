using UnityEngine;

public class FireballAbilityExecutor : CombatActionExecutor
{
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed = 15f;
    [SerializeField] private float fireballHeightOffset = 1.2f;
    [SerializeField] private float defaultTimeToAttack = 1f;
    [SerializeField] private float attackAnimationTime = 1f;
    [SerializeField] private float attackSpawnProgress = 0.95f;

    private float cooldownTimer;
    private float animationTimer;
    private bool isAttackLocked;
    private bool isCasting;
    private Vector3 targetPosition;

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

        if (!isCasting) return;

        animationTimer -= Time.deltaTime;

        float progress = 1f - (animationTimer / attackAnimationTime);

        isAttackLocked = progress >= 0.3f && progress <= 0.6f;

        if (progress >= attackSpawnProgress)
        {
            SpawnFireball(targetPosition);
            cooldownTimer = ApplyCooldown(defaultTimeToAttack);
            isCasting = false;
            isAttackLocked = false;
            SetPerformingCombatAction(false);
        }
        else
        {
            SetPerformingCombatAction(isAttackLocked);
        }

        if (animationTimer <= 0f)
        {
            isAttackLocked = false;
            SetPerformingCombatAction(false);
        }
    }

    public override void Execute(Command command)
    {
        if (command.target != null)
            targetPosition = command.target.transform.position + Vector3.up * fireballHeightOffset;
        else
            targetPosition = transform.position + transform.forward * 10f + Vector3.up * fireballHeightOffset;

        StopMovement();
        RotateTowardsPoint(targetPosition);
        animationTimer = attackAnimationTime;
        isAttackLocked = false;
        isCasting = true;
        TriggerAttackAnimation();
    }

    private void SpawnFireball(Vector3 targetPos)
    {
        if (fireballPrefab == null) return;

        Vector3 spawnPos = transform.position + Vector3.up * fireballHeightOffset;
        Vector3 flatTarget = new Vector3(targetPos.x, spawnPos.y, targetPos.z);
        Vector3 dir = (flatTarget - spawnPos).normalized;

        GameObject proj = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
        Fireball fireball = proj.GetComponent<Fireball>();
        if (fireball != null)
            fireball.Initialize(character, dir, fireballSpeed, fireballHeightOffset);
    }

    private void TriggerAttackAnimation()
    {
        if (AnimatorHasTrigger("SpellCast"))
            animator.SetTrigger("SpellCast");
    }

    public override void ResetState()
    {
        isCasting = false;
        animationTimer = 0f;
        isAttackLocked = false;
        SetPerformingCombatAction(false);
        ResetAnimatorTriggers("SpellCast");
    }

    protected override float ApplyCooldown(float baseCooldown)
    {
        return baseCooldown;
    }
}
