using UnityEngine;

public class CommandHandler : MonoBehaviour
{
    public Command CurrentCommand { get; private set; }

    private CommandProcessor processor;

    private void Awake()
    {
        processor = new CommandProcessor(
            GetComponent<MoveHandler>(),
            GetComponent<AttackHandler>(),
            GetComponent<InteractHandler>()
        );
    }

    public void SetCommand(Command newCommand)
    {
        CurrentCommand = newCommand;
    }

    private void Update()
    {
        if (CurrentCommand == null) return;

        Character character = GetComponent<Character>();
        if (character == null || character.IsDead)
        {
            ClearCurrentCommand();
            return;
        }

        processor.Process(CurrentCommand);

        if (CurrentCommand.isComplete)
        {
            ClearCurrentCommand();
        }
    }

    public CommandType GetCurrentCommandType()
    {
        return CurrentCommand?.commandType ?? CommandType.None;
    }

    public void ClearCurrentCommand()
    {
        CurrentCommand = null;
    }
}
