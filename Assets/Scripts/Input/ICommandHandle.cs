using UnityEngine;

namespace CharacterCommand
{
    public interface ICommandHandle
    {
        public void ProcessCommand(Command command);
    }
}
