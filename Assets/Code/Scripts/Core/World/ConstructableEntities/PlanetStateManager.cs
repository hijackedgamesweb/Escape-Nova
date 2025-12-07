using System;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.World.ConstructableEntities.States;
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
        private Planet _planet;

        public PlanetStateManager(IGameTime gameTime, Planet planet)
        {
            _gameTime = gameTime;
            _planet = planet;
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
            var type = Type.GetType(stateName);
            if (type == null)
                throw new Exception($"State type '{stateName}' not found.");

            IState state;

            // crear siempre con Planet y GameTime
            state = Activator.CreateInstance(type, _planet, _gameTime) as IState;

            if (state == null)
                throw new Exception($"Could not create instance of state type '{stateName}'.");

            SetState(state);

            // reconectar eventos si es un BuildingState restaurado
            if (state is BuildingState bs)
            {
                bs.OnProgressUpdated += _planet.HandleBuildingProgress;
            }
            else
            {
                _planet.CompleteConstructionInstantly();
            }
        }
    }
}