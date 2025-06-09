using System;
using CharacterCommand;
using UnityEngine;

[RequireComponent(typeof(CommandHandler))]
public class AIEnemy : MonoBehaviour
{
    [SerializeField] AIAgentGroup aiGroup;

    CommandHandler commandHandler;
    Character character;

    [SerializeField] float attackRange = 5f;

    private void Awake()
    {
        commandHandler = GetComponent<CommandHandler>();
        character = GetComponent<Character>();
    }

    float timer = 0.2f;

    private void Start()
    {
        aiGroup.Add(this);
    }

    private void OnDestroy()
    {
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.EditorApplication.isPlaying == false)
        {
            return;
        }

        if (aiGroup != null)
        {
            aiGroup.Remove(this);
        }
    }

    internal void UpdateAgent(GameObject targetToAttack)
    {
        timer -= Time.deltaTime;

        if (character == null || character.lifePool.currentValue <= 0)
        {
            if (commandHandler != null)
            {
                commandHandler.SetCommand(null);
            }

            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, targetToAttack.transform.position);

        if (targetToAttack.GetComponent<Character>() == null || targetToAttack.GetComponent<Character>().lifePool.currentValue > 0)
        {
            if (timer < 0f && distanceToTarget <= attackRange)
            {
                timer = 0.2f;

                commandHandler.SetCommand(new Command(CommandType.Attack, targetToAttack));
            }
        }
        else
        {
            commandHandler.SetCommand(null);
        }
    }
}
