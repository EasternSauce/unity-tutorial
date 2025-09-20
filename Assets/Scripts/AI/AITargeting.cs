using System.Collections.Generic;
using UnityEngine;

public class AITargeting : MonoBehaviour
{
    private AIAggro aggro;

    private void Awake()
    {
        aggro = GetComponent<AIAggro>();
    }

    public void SearchForTargets()
    {
        Character player = FindClosestLivingPlayer();
        if (player != null && IsWithinAggroDistance(player))
        {
            aggro.GainAggro(player.gameObject);
        }
    }

    private Character FindClosestLivingPlayer()
    {
        List<Character> players = CharacterUtils.GetPlayerCharacters();
        Character closest = null;
        float minDist = float.MaxValue;

        foreach (var p in players)
        {
            if (p.IsDead || !p.IsPlayer) continue;
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = p;
            }
        }
        return closest;
    }

    private bool IsWithinAggroDistance(Character player)
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance <= aggro.GetAggroDistance();
    }
}
