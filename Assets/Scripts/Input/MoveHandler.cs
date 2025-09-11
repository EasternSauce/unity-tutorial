using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CanMoveState))]
public class MoveHandler : MonoBehaviour, ICommandHandle
{
    [SerializeField] private float defaultStoppingDistance = 0.1f;
    [SerializeField] private NavMeshAgent agent;

    public NavMeshAgent Agent => agent;
    public float DefaultStoppingDistance => defaultStoppingDistance;

    private Character character;
    [SerializeField] private float default_MoveSpeed = 3.5f;
    private StatsValue moveSpeed;
    private CanMoveState canMoveState;

    private Command currentCommand;
    public Command CurrentCommand => currentCommand;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<Character>();
        canMoveState = GetComponent<CanMoveState>();
    }

    private void Start()
    {
        moveSpeed = character.GetStatsValue(Statistic.MoveSpeed);
        ApplyMoveSpeed();
    }

    private void Update()
    {
        if (moveSpeed == null) return;

        // keep speed in sync every frame (no guard!)
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

        // only apply if valid
        if (agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.speed = newSpeed;
    }

    public void SetDestination(Vector3 destinationPosition)
    {
        if (canMoveState.Check() == true)
        {
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(destinationPosition);
            }
        }
    }

    public void Stop()
    {
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }

    public void ProcessCommand(Command command)
    {
        SetDestination(command.worldPoint);
        currentCommand = command;
    }
}
