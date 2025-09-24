using System.Collections;
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
    private Coroutine localCoroutine;

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
        if (animationTimer > 0f) animationTimer -= Time.deltaTime;

        if (animationTimer <= 0f) isAttackLocked = false;
        else
        {
            float progress = 1f - (animationTimer / attackAnimationTime);
            if (!isAttackLocked && progress >= 0.3f && progress <= 0.6f) isAttackLocked = true;
            else if (isAttackLocked && progress > 0.6f) isAttackLocked = false;
        }

        SetPerformingCombatAction(isAttackLocked);
    }

    public override void Execute(Command command)
    {
        Vector3 targetPos = command != null && command.target != null
            ? command.target.transform.position + Vector3.up * fireballHeightOffset
            : transform.position + transform.forward * 10f;

        CastFireballAtPosition(targetPos, gameObject);
    }

    public void CastFireballAtPosition(Vector3 targetPos, GameObject shooter)
    {
        if (cooldownTimer > 0f || localCoroutine != null) return;

        StopMovement();
        RotateTowardsPoint(targetPos);
        SetPerformingCombatAction(true);

        animationTimer = attackAnimationTime;
        isAttackLocked = false;
        TriggerAttackAnimation();

        float delay = attackAnimationTime * attackSpawnProgress;
        localCoroutine = StartCoroutine(SpawnFireballDelayed(targetPos, shooter, delay));
    }

    private IEnumerator SpawnFireballDelayed(Vector3 targetPos, GameObject shooter, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 spawnPos = transform.position + Vector3.up * fireballHeightOffset;
        Vector3 flatTarget = new Vector3(targetPos.x, spawnPos.y, targetPos.z);
        Vector3 dir = (flatTarget - spawnPos).normalized;

        if (fireballPrefab != null)
        {
            GameObject proj = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
            Fireball fireball = proj.GetComponent<Fireball>();
            if (fireball != null)
            {
                Character shooterChar = shooter.GetComponent<Character>();
                if (shooterChar != null)
                    fireball.Initialize(shooterChar, dir, fireballSpeed, fireballHeightOffset);
            }
        }

        cooldownTimer = ApplyCooldown(defaultTimeToAttack);
        StopAndClearCoroutine(ref localCoroutine);
        isAttackLocked = false;
        SetPerformingCombatAction(false);
    }

    private void TriggerAttackAnimation()
    {
        if (AnimatorHasTrigger("SpellCast")) animator.SetTrigger("SpellCast");
    }

    public override void ResetState()
    {
        base.ResetState();
        StopAndClearCoroutine(ref localCoroutine);
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
