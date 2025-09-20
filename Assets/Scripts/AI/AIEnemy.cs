using System.Collections.Generic;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    [Header("Aggro Settings")]
    [SerializeField] private float aggroDistance = 5f;
    [SerializeField] private float aggroLoseDistance = 7f;
    [SerializeField] private float aggroLoseTime = 3f;

    private GameObject currentTarget;
    private float timeOutsideAggro;
    private bool isAggroed;

    private MoveCommandHandler moveHandler;
    private AttackCommandHandler attackHandler;
    private Character selfCharacter;

    private void Awake()
    {
        moveHandler = GetComponent<MoveCommandHandler>();
        attackHandler = GetComponent<AttackCommandHandler>();
        selfCharacter = GetComponent<Character>();
    }

    private void Update()
    {
        if (!CanAct())
        {
            DropAggro();
            return;
        }

        if (HasTarget())
        {
            HandleTarget();
        }
        else
        {
            SearchForTargets();
        }
    }

    private bool CanAct()
    {
        return selfCharacter != null && !selfCharacter.IsDead;
    }

    private bool HasTarget()
    {
        return currentTarget != null;
    }

    private Character GetTargetCharacter()
    {
        return currentTarget != null ? currentTarget.GetComponent<Character>() : null;
    }

    private void HandleTarget()
    {
        var targetCharacter = GetTargetCharacter();
        if (targetCharacter == null || targetCharacter.IsDead)
        {
            DropAggro();
            return;
        }

        if (IsTargetOutOfRange())
        {
            UpdateAggroTimer();
        }
        else
        {
            ResetAggroTimer();
        }

        if (isAggroed)
        {
            AttackTarget();
        }
    }

    private bool IsTargetOutOfRange()
    {
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        return distance > aggroLoseDistance;
    }

    private void UpdateAggroTimer()
    {
        timeOutsideAggro += Time.deltaTime;
        if (timeOutsideAggro >= aggroLoseTime)
        {
            DropAggro();
        }
    }

    private void ResetAggroTimer()
    {
        timeOutsideAggro = 0f;
    }

    private void AttackTarget()
    {
        attackHandler?.ProcessCommand(new Command(CommandType.Attack, currentTarget));
    }

    private void SearchForTargets()
    {
        Character player = FindClosestLivingPlayer();
        if (player != null && IsWithinAggroDistance(player))
        {
            GainAggro(player.gameObject);
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
        return distance <= aggroDistance;
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
        if (attacker == null || currentTarget == attacker) return;
        GainAggro(attacker);
    }
}
