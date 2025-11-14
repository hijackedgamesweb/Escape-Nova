using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Time;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.State.Interfaces;
using UnityEngine;

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
            Debug.Log($"BuildingState: {_planetData.Name} started construction ({_planetData.TimeToBuild} cycles)");
        }

        public void Exit(IStateManager gameManager)
        {
            _gameTime.OnCycleCompleted -= UpdateCycle;
        }

        public void UpdateCycle(int currentCycle)
        {
            _cycleCount++;
            Debug.Log($"BuildingState: {_planetData.Name} construction progress {_cycleCount}/{_planetData.TimeToBuild}");

            if (_cycleCount >= _planetData.TimeToBuild)
            {
                // Cambiar al estado de producción
                gameManager.SetState(new ProductionState(_planetData, _gameTime));
                Debug.Log($"BuildingState: {_planetData.Name} construction completed");
            }
        }

        public void Update()
        {
        }
    }
}