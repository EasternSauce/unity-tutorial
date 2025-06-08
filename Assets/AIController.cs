using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] AIAgentGroup agentGroup;
    [SerializeField] GameObject targetToAttack;

    private void Update()
    {
        if (agentGroup.agents == null) { return; }

        for (int i = 0; i < agentGroup.agents.Count; i++)
        {
            agentGroup.agents[i].UpdateAgent(targetToAttack);
        }
    }
}
