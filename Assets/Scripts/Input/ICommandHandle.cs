namespace CharacterCommand
{
    public interface ICommandHandle
    {
        void ProcessCommand(Command command);

        void CancelCommand();
    }
}
