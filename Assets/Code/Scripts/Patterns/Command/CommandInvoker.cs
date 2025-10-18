using System.Collections.Generic;
using Code.Scripts.Patterns.Command.Interfaces;

namespace Code.Scripts.Patterns.Command
{
    public class CommandInvoker 
    {
        private Stack<ICommand> _commandHistory = new Stack<ICommand>();
        
        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _commandHistory.Push(command);
        }
        
        public void Undo()
        {
            if (_commandHistory.Count > 0)
            {
                ICommand command = _commandHistory.Pop();
                command.Undo();
            }
        }
    }
}