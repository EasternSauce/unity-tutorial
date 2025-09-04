using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SafeNavMeshAgentActivator : MonoBehaviour
{
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        if (NavMesh.SamplePosition(transform.position, out _, 1f, NavMesh.AllAreas))
        {
            agent.enabled = true;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: No valid NavMesh at start position. Agent not enabled.");
        }
    }
}