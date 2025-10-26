namespace Code.Scripts.Patterns.State.Interfaces
{
    public interface IStateManager
    {
        public IState GetCurrentState();
        public void SetState(IState state);
        
    }
}