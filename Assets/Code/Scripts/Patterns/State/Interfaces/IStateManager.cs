namespace Code.Scripts.Patterns.State.Interfaces
{
    public interface IStateManager
    {
        
        public void StartGame();
        public void EndGame();
        public IState GetCurrentState();
        public void SetState(IState state);

        public void SetState<T>() where T : AState;
    }
}