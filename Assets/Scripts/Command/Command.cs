using UnityEngine;

public enum CommandType
{
    None,
    Move,
    CombatAction,
    Interact
}

public class Command
{
    public CommandType commandType;
    public Vector3 worldPoint;
    public GameObject target;
    public bool isComplete;

    public Command(CommandType commandType, Vector3 worldPoint)
    {
        this.commandType = commandType;
        this.worldPoint = worldPoint;
    }

    public Command(CommandType commandType, GameObject target)
    {
        this.commandType = commandType;
        this.target = target;
    }
}
