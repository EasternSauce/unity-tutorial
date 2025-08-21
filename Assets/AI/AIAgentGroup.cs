using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AIAgentGroup : ScriptableObject
{
    public List<AIEnemy> agents;

    public void Add(AIEnemy agent)
    {
        if (agents == null)
        {
            agents = new List<AIEnemy>();
        }

        agents.Add(agent);
    }

    public void Remove(AIEnemy agent)
    {
        agents.Remove(agent);
    }
}
