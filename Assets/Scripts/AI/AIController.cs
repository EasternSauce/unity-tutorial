using UnityEngine;

public class AIController : MonoBehaviour
{
    private AIAggro aggro;
    private AITargeting targeting;
    private AICombat combat;
    private Character selfCharacter;

    private void Awake()
    {
        aggro = GetComponent<AIAggro>();
        targeting = GetComponent<AITargeting>();
        combat = GetComponent<AICombat>();
        selfCharacter = GetComponent<Character>();
    }

    private void Update()
    {
        if (!CanAct())
        {
            aggro.DropAggro();
            combat.StopCombat();
            return;
        }

        if (aggro.HasTarget())
        {
            combat.HandleTarget(aggro.CurrentTarget);
        }
        else
        {
            targeting.SearchForTargets();
        }
    }

    private bool CanAct()
    {
        return selfCharacter != null && !selfCharacter.IsDead;
    }

    public void OnAttacked(GameObject attacker)
    {
        if (attacker == null || aggro.CurrentTarget == attacker) return;
        aggro.GainAggro(attacker);
    }
}
