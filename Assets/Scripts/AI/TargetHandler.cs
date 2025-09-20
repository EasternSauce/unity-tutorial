using UnityEngine;
using System.Collections.Generic;

public class TargetHandler : MonoBehaviour
{
    private Character selfCharacter;
    private AggroController aggroController;

    private void Awake()
    {
        selfCharacter = GetComponent<Character>();
        aggroController = GetComponent<AggroController>();
    }

    public Character GetTargetCharacter()
    {
        if (aggroController.CurrentTarget == null) return null;
        return aggroController.CurrentTarget.GetComponent<Character>();
    }

    public void SearchForTargets()
    {
        Character player = FindClosestLivingPlayer();
        if (player != null && aggroController.IsWithinAggroDistance(player.gameObject))
        {
            aggroController.GainAggro(player.gameObject);
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

    public bool HasValidTarget()
    {
        Character target = GetTargetCharacter();
        return target != null && !target.IsDead;
    }
}
