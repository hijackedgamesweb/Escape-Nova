using System;
using Code.Scripts.Core.Managers.Interfaces;
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
        
        public event Action<float> OnProgressUpdated;

        public BuildingState(Planet planetData, IGameTime gameTime)
        {
            _gameTime = gameTime;
            _planetData = planetData;
        }

        public void Enter(IStateManager gameManager)
        {
            _gameTime.OnCycleCompleted += UpdateCycle;
            this.gameManager = gameManager;
            OnProgressUpdated?.Invoke(0f);
        }

        public void Exit(IStateManager gameManager)
        {
            _gameTime.OnCycleCompleted -= UpdateCycle;
        }

        public void UpdateCycle(int currentCycle)
        {
            _cycleCount++;
            float progress = Mathf.Clamp01((float)_cycleCount / _planetData.TimeToBuild);
            OnProgressUpdated?.Invoke(progress);

            if (_cycleCount >= _planetData.TimeToBuild)
            {
                OnProgressUpdated?.Invoke(1f);
                
                gameManager.SetState(new ProductionState(_planetData, _gameTime));
            }
        }

        public void Update()
        {
        }
    }
}