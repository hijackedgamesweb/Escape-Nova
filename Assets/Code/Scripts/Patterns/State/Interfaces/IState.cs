namespace Code.Scripts.Patterns.State.Interfaces
{
    public interface IState
    {
        public void Enter(IStateManager gameManager);
        public void Exit(IStateManager gameManager);
        public void Update();
    }
}