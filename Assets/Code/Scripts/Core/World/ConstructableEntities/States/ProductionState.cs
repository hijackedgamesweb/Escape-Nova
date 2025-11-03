using System.Resources;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.Systems.Time;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.State.Interfaces;

namespace Code.Scripts.Core.World.ConstructableEntities.States
{
    public class ProductionState : IState
    {
        IStateManager gameManager;
        IGameTime _gameTime;
        int _cycleCount = 0;
        PlanetDataSO _planetData;
        TimeScheduler _timeScheduler;
        StorageSystem _storageSystem;
        public ProductionState(PlanetDataSO planetData, IGameTime gameTime)
        {
            _gameTime = gameTime;
            _planetData = planetData;
        }
        public void Enter(IStateManager gameManager)
        {
            _timeScheduler = ServiceLocator.GetService<TimeScheduler>();
            _timeScheduler.ScheduleRepeating(1, ProduceResources);
        }

        public void Exit(IStateManager gameManager)
        {
            _timeScheduler.CancelAllForTarget(ProduceResources);
        }

        private void ProduceResources()
        {
            for (int i = 0; i < _planetData.producibleResources.Count; i++)
            {
                _storageSystem = ServiceLocator.GetService<StorageSystem>();
                _storageSystem.AddResource(_planetData.producibleResources[i].Type, _planetData.resourcePerCycle[i]);
            }
        }

        public void Update()
        {
        }
    }
}