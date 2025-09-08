
public class CommandProcessor
{
    private readonly ICommandHandle moveHandler;
    private readonly ICommandHandle attackHandler;
    private readonly ICommandHandle interactHandler;

    public CommandProcessor(
        ICommandHandle moveHandler,
        ICommandHandle attackHandler,
        ICommandHandle interactHandler)
    {
        this.moveHandler = moveHandler;
        this.attackHandler = attackHandler;
        this.interactHandler = interactHandler;
    }

    public void Process(Command command)
    {
        switch (command.commandType)
        {
            case CommandType.Move:
                moveHandler.ProcessCommand(command);
                break;
            case CommandType.Attack:
                attackHandler.ProcessCommand(command);
                break;
            case CommandType.Interact:
                interactHandler.ProcessCommand(command);
                break;
        }
    }
}
