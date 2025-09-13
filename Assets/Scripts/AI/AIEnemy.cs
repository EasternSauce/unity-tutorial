using System.Collections.Generic;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    [SerializeField] private float aggroDistance = 5f;
    [SerializeField] private float aggroLoseDistance = 7f;
    [SerializeField] private float aggroLoseTime = 3f;

    private GameObject currentTarget;
    private float timeOutsideAggro;
    private bool isAggroed;

    private MoveHandler moveHandler;
    private AttackHandler attackHandler;
    private Character character;

    private void Awake()
    {
        moveHandler = GetComponent<MoveHandler>();
        attackHandler = GetComponent<AttackHandler>();
        character = GetComponent<Character>();
    }

    private void Update()
    {
        if (character == null || character.IsDead)
        {
            DropAggro();
            return;
        }

        if (currentTarget != null && !currentTarget.GetComponent<Character>().IsDead)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distance > aggroLoseDistance)
            {
                timeOutsideAggro += Time.deltaTime;
                if (timeOutsideAggro >= aggroLoseTime)
                    DropAggro();
            }
            else
            {
                timeOutsideAggro = 0f;
            }

            if (isAggroed)
            {
                attackHandler?.ProcessCommand(new Command(CommandType.Attack, currentTarget));
            }
        }
        else
        {
            SearchForTargets();
        }
    }

    private void SearchForTargets()
    {
        Character player = FindClosestPlayer();
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= aggroDistance)
            {
                GainAggro(player.gameObject);
            }
        }
    }

    private Character FindClosestPlayer()
    {
        List<Character> players = CharacterUtils.GetPlayerCharacters();
        Character closest = null;
        float minDist = float.MaxValue;
        foreach (var p in players)
        {
            if (p.IsDead) continue;
            if (!p.IsPlayer) continue;
            float dist = Vector3.Distance(transform.position, p.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = p;
            }
        }
        return closest;
    }

    public void GainAggro(GameObject target)
    {
        currentTarget = target;
        isAggroed = true;
        timeOutsideAggro = 0f;
    }

    private void DropAggro()
    {
        currentTarget = null;
        isAggroed = false;
        timeOutsideAggro = 0f;
        moveHandler?.Stop();
        attackHandler?.CancelAttack();
    }

    public void OnAttacked(GameObject attacker)
    {
        if (attacker == null) return;
        if (currentTarget == attacker) return;
        GainAggro(attacker);
    }
}
