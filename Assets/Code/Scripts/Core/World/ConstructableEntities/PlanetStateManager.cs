using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.State.Interfaces;
using Newtonsoft.Json.Linq;

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
            _currentState?.Enter(this);
        }

        public JToken GetCurrentStateName()
        {
            return _currentState != null ? JToken.FromObject(_currentState.GetType().Name) : JValue.CreateNull();
        }

        public void SetStateByName(string stateName)
        {
            var stateType = System.Type.GetType(stateName);
            if (stateType == null)
            {
                throw new System.Exception($"State type '{stateName}' not found.");
            }

            var stateInstance = System.Activator.CreateInstance(stateType, _gameTime) as IState;
            if (stateInstance == null)
            {
                throw new System.Exception($"Could not create instance of state type '{stateName}'.");
            }

            SetState(stateInstance);
        }
    }
}