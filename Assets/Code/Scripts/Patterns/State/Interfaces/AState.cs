using Code.Scripts.Core.Managers;

namespace Code.Scripts.Patterns.State.Interfaces
{
    public abstract class AState : IState
    {
        protected IStateManager _stateManager;
        
        public AState (IStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public abstract void Enter(IStateManager gameManager);
        public abstract void Exit(IStateManager gameManager);
        public abstract void Update();
    }
}