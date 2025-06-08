using System;
using CharacterCommand;
using UnityEngine;

[RequireComponent(typeof(CommandHandler))]
public class AIEnemy : MonoBehaviour
{
    [SerializeField] AIAgentGroup aiGroup;

    CommandHandler commandHandler;

    [SerializeField] float attackRange = 5f;

    private void Awake()
    {
        commandHandler = GetComponent<CommandHandler>();
    }

    float timer = 0.2f;

    private void Start()
    {
        aiGroup.Add(this);
    }

    private void OnDestroy()
    {
        aiGroup.Remove(this);
    }

    internal void UpdateAgent(GameObject targetToAttack)
    {
        timer -= Time.deltaTime;
        float distanceToTarget = Vector3.Distance(transform.position, targetToAttack.transform.position);

        if ((targetToAttack.GetComponent<Character>() == null || targetToAttack.GetComponent<Character>().lifePool.currentValue > 0) && timer < 0f && distanceToTarget <= attackRange)
        {
            timer = 0.2f;

            commandHandler.SetCommand(new Command(CommandType.Attack, targetToAttack));
        }
    }
}
