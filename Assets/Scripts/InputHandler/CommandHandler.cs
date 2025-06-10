using System;
using UnityEngine;

namespace CharacterCommand
{
    public enum CommandType
    {
        None,
        Move,
        Attack,
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

    public class CommandHandler : MonoBehaviour
    {
        public Command currentCommand;
        ICommandHandle moveCommandHandler;
        ICommandHandle attackCommandHandler;
        ICommandHandle interactCommandHandler;

        private void Awake()
        {
            moveCommandHandler = GetComponent<CharacterMovement>();
            attackCommandHandler = GetComponent<AttackHandler>();
            interactCommandHandler = GetComponent<InteractHandler>();
        }

        public void SetCommand(Command newCommand)
        {
            currentCommand = newCommand;
        }

        private void Update()
        {
            if (currentCommand == null) return;

            Character character = GetComponent<Character>();
            if (character == null || character.IsDead)
            {
                currentCommand = null;
                return;
            }

            ProcessCommand();
        }

        private void ProcessCommand()
        {
            switch (currentCommand.commandType)
            {
                case CommandType.Move:
                    ProcessMoveCommand();
                    break;
                case CommandType.Attack:
                    ProcessAttackCommand();
                    break;
                case CommandType.Interact:
                    ProcessInteractCommand();
                    break;
            }
            if (currentCommand.isComplete)
            {
                CompleteCommand();
            }
        }

        private void CompleteCommand()
        {
            currentCommand = null;
        }

        private void ProcessInteractCommand()
        {
            interactCommandHandler.ProcessCommand(currentCommand);
        }

        private void ProcessAttackCommand()
        {
            attackCommandHandler.ProcessCommand(currentCommand);

        }

        private void ProcessMoveCommand()
        {
            moveCommandHandler.ProcessCommand(currentCommand);
        }

        public CommandType GetCurrentCommandType()
        {
            if (currentCommand == null)
            {
                return CommandType.None;
            }

            return currentCommand.commandType;
        }
    }

}

