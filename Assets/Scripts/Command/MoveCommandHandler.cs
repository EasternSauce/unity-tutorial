using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Character))]
public class MoveCommandHandler : MonoBehaviour, ICommandHandler
{
    [SerializeField] private float defaultStoppingDistance = 0.1f;
    [SerializeField] private NavMeshAgent agent;
    private Character character;
    [SerializeField] private float default_MoveSpeed = 3.5f;
    private RegularStatValue moveSpeed;
    private Command currentCommand;

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

    public void MoveTo(Vector3 position, float stoppingDistance = -1f)
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;
        if (stoppingDistance >= 0f) agent.stoppingDistance = stoppingDistance;
        agent.isStopped = false;
        agent.SetDestination(position);
    }

    public void Stop()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;
        agent.isStopped = true;
    }

    public void RotateTowards(Vector3 point)
    {
        if (character == null) return;
        Vector3 direction = (point - character.transform.position).normalized;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            character.transform.rotation = Quaternion.Slerp(character.transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    public float RemainingDistance => agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh ? agent.remainingDistance : 0f;
    public bool IsOnNavMesh => agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh;

    public void ProcessCommand(Command command)
    {
        currentCommand = command;
        MoveTo(command.worldPoint, defaultStoppingDistance);
    }
}
