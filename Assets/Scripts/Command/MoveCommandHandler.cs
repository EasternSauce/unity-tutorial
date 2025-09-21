using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Character))]
public class MoveCommandHandler : MonoBehaviour, ICommandHandler
{
    [SerializeField] private float defaultStoppingDistance = 0.1f;
    [SerializeField] private NavMeshAgent agent;

    public NavMeshAgent Agent => agent;
    public float DefaultStoppingDistance => defaultStoppingDistance;

    private Character character;
    [SerializeField] private float default_MoveSpeed = 3.5f;
    private RegularStatValue moveSpeed;

    private Command currentCommand;
    public Command CurrentCommand => currentCommand;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<Character>();
    }

    private void Start()
    {
        moveSpeed = character.GetStatsValue(RegularStat.MoveSpeed);
        ApplyMoveSpeed();
    }

    private void Update()
    {
        if (moveSpeed == null) return;

        ApplyMoveSpeed();

        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            if (currentCommand != null)
            {
                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    currentCommand.isComplete = true;
                    currentCommand = null;
                }
            }
        }
    }

    private void ApplyMoveSpeed()
    {
        if (agent == null) return;

        float newSpeed = default_MoveSpeed * moveSpeed.float_value;
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.speed = newSpeed;
    }

    public void SetDestination(Vector3 destinationPosition)
    {
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(destinationPosition);
        }
    }

    public void Stop()
    {
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.isStopped = true;
    }

    public void ProcessCommand(Command command)
    {
        SetDestination(command.worldPoint);
        currentCommand = command;
    }
}
