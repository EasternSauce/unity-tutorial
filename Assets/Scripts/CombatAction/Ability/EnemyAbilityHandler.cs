using UnityEngine;

public class EnemyAbilityHandler : MonoBehaviour
{
    [SerializeField] private Ability magicAbility;
    private CombatActionController combatActionController;
    private float cooldownTimer;

    private void Awake()
    {
        combatActionController = GetComponent<CombatActionController>();
    }

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
        if (!CanCast() || target == null) return;
        combatActionController.Execute(CombatActionType.Fireball, new Command(CommandType.CombatAction, target));
        cooldownTimer = magicAbility.cooldown;
    }
}
