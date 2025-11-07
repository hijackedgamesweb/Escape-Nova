using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Time;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.State.Interfaces;

namespace Code.Scripts.Core.World.ConstructableEntities.States
{
    public class BuildingState : IState
    {
        IStateManager gameManager;
        IGameTime _gameTime;
        int _cycleCount = 0;
        Planet _planetData;
        public BuildingState(Planet planetData, IGameTime gameTime)
        {
            _gameTime = gameTime;
            _planetData = planetData;
        }
        
        public void Enter(IStateManager gameManager)
        {
            _gameTime.OnCycleCompleted += UpdateCycle;
            this.gameManager = gameManager;
        }

        public void Exit(IStateManager gameManager)
        {
            _gameTime.OnCycleCompleted -= UpdateCycle;
        }

        public void UpdateCycle(int currentCycle)
        {
            _cycleCount++;
            if (_cycleCount >= _planetData.TimeToBuild)
            {
                gameManager.SetState(new ProductionState(_planetData, _gameTime));
            }
        }
        public void Update()
        {
        }
    }
}