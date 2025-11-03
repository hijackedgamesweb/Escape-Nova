using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.State.Interfaces;

namespace Code.Scripts.Core.World.ConstructableEntities
{
    public class PlanetStateManager : IStateManager
    {
        
        IState _currentState;
        IGameTime _gameTime;
        public IState GetCurrentState()
        {
            return _currentState;
        }
        
        public PlanetStateManager(IGameTime gameTime)
        {
            _gameTime = gameTime;
        }

        public void SetState(IState state)
        {
            _currentState?.Exit(this);
            _currentState = state;
            _currentState.Enter(this);
        }
    }
}