using System;
using CharacterCommand;
using UnityEngine;

[RequireComponent(typeof(CommandHandler))]
[RequireComponent(typeof(Character))]
public class AIEnemy : MonoBehaviour
{
    [SerializeField] AIAgentGroup aiGroup;

    private CommandHandler commandHandler;
    private Character character;

    [SerializeField] private float attackRange = 5f;

    private float timer = 0.2f;

    private void Awake()
    {
        commandHandler = GetComponent<CommandHandler>();
        character = GetComponent<Character>();
    }

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

        if (character == null || character.IsDead)
        {
            if (commandHandler != null)
            {
                commandHandler.SetCommand(null);
            }

            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, targetToAttack.transform.position);

        var targetCharacter = targetToAttack.GetComponent<Character>();
        if (targetCharacter != null && !targetCharacter.IsDead)
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
