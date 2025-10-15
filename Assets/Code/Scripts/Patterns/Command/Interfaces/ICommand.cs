namespace Code.Scripts.Patterns.Command.Interfaces
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}