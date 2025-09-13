using UnityEngine;

[RequireComponent(typeof(CommandHandler))]
[RequireComponent(typeof(Character))]
public class AIEnemy : MonoBehaviour
{
    private CommandHandler commandHandler;
    private Character character;

    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 0.2f;
    private float timer;

    [SerializeField] private GameObject targetToAttack;

    private void Awake()
    {
        commandHandler = GetComponent<CommandHandler>();
        character = GetComponent<Character>();
        timer = attackCooldown;
    }

    private void Update()
    {
        UpdateAgent();
    }

    private void UpdateAgent()
    {
        timer -= Time.deltaTime;

        if (targetToAttack != null && targetToAttack.GetComponent<Character>()?.IsDead == true)
        {
            targetToAttack = null;
            commandHandler?.CancelCurrentCommand();
        }

        if (targetToAttack == null)
            FindClosestTarget();

        if (character == null || character.IsDead || targetToAttack == null)
            return;

        float distanceToTarget = Vector3.Distance(transform.position, targetToAttack.transform.position);

        if (timer <= 0f && distanceToTarget <= attackRange)
        {
            timer = attackCooldown;
            commandHandler.ExecuteCommand(new Command(CommandType.Attack, targetToAttack));
        }
    }

    private void FindClosestTarget()
    {
        Character[] allCharacters = Object.FindObjectsByType<Character>(FindObjectsSortMode.None);
        float closestDistance = float.MaxValue;
        GameObject closest = null;

        foreach (var c in allCharacters)
        {
            if (c == character) continue;
            if (c.IsDead) continue;
            if (c.GetComponent<AIEnemy>() != null) continue;

            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = c.gameObject;
            }
        }

        targetToAttack = closest;
    }
}
