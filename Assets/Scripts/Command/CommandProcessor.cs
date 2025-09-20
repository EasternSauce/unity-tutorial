
public class CommandProcessor
{
    private readonly ICommandHandler moveHandler;
    private readonly ICommandHandler attackHandler;
    private readonly ICommandHandler interactHandler;

    public CommandProcessor(
        ICommandHandler moveHandler,
        ICommandHandler attackHandler,
        ICommandHandler interactHandler)
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
