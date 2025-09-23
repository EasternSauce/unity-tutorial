using UnityEngine;

public class EnemyAbilityHandler : MonoBehaviour
{
    [SerializeField] private Ability magicAbility;
    [SerializeField] private FireballAbilityExecutor fireballExecutor;
    private float cooldownTimer;

    private void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    public bool CanCast()
    {
        return cooldownTimer <= 0f && magicAbility != null;
    }

    public void CastMagic(GameObject target)
    {
        if (!CanCast() || fireballExecutor == null || target == null) return;

        Vector3 targetPos = target.transform.position;
        fireballExecutor.CastFireballAtPosition(targetPos, gameObject);
        cooldownTimer = magicAbility.cooldown;
    }
}
