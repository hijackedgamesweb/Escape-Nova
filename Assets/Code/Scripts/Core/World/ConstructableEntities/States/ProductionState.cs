using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.State.Interfaces;
using UnityEngine;

namespace Code.Scripts.Core.World.ConstructableEntities.States
{
    public class ProductionState : IState
    {
        IStateManager gameManager;
        IGameTime _gameTime;
        Planet _planetData;
        StorageSystem _storageSystem;
        private int _lastCycleProduced = -1;

        public ProductionState(Planet planetData, IGameTime gameTime)
        {
            _gameTime = gameTime;
            _planetData = planetData;
        }

        public void Enter(IStateManager gameManager)
        {
            this.gameManager = gameManager;

            // Suscribirse a los ciclos en lugar de usar TimeScheduler
            _gameTime.OnCycleCompleted += OnCycleCompleted;

            // Obtener el StorageSystem
            _storageSystem = WorldManager.Instance.Player.StorageSystem;

            // Inicializar el último ciclo producido
            _lastCycleProduced = _gameTime.CurrentCycle;

            // Notificar sobre la producción (para UI u otros sistemas)
            for (int i = 0; i < _planetData.ProducibleResources.Count; i++)
            {
                ConstructionEvents.OnResourceProductionAdded?.Invoke(_planetData.ResourcePerCycle[i], _planetData.ProducibleResources[i]);
            }

        }

        public void Exit(IStateManager gameManager)
        {
            // Desuscribirse de los ciclos
            _gameTime.OnCycleCompleted -= OnCycleCompleted;

            // Notificar que se detiene la producción
            for (int i = 0; i < _planetData.ProducibleResources.Count; i++)
            {
                ConstructionEvents.OnResourceProductionAdded?.Invoke(-_planetData.ResourcePerCycle[i], _planetData.ProducibleResources[i]);
            }

        }

        private void OnCycleCompleted(int currentCycle)
        {
            // Producir recursos una vez por ciclo
            if (_lastCycleProduced < currentCycle)
            {
                ProduceResources();
                _lastCycleProduced = currentCycle;
            }
        }

        private void ProduceResources()
        {
            if (_planetData.ProducibleResources == null || _planetData.ResourcePerCycle == null)
                return;

            for (int i = 0; i < _planetData.ProducibleResources.Count; i++)
            {
                if (i < _planetData.ResourcePerCycle.Length && _planetData.ResourcePerCycle[i] > 0)
                {
                    ResourceType resourceType = _planetData.ProducibleResources[i];
                    int amount = _planetData.ResourcePerCycle[i];

                    // Agregar recursos al sistema de almacenamiento
                    if (_storageSystem != null)
                    {
                        _storageSystem.AddResource(resourceType, amount);
                    }
                }
            }
        }

        public void Update()
        {
            // No necesita actualización por frame, la producción es por ciclos
        }
    }
}