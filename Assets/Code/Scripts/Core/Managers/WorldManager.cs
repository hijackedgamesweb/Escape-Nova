using System;
using System.Collections.Generic;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Entity.Player;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Civilization;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.Singleton;
using Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class WorldManager : Singleton<WorldManager>
    {
        //STORAGE SYSTEM
        [SerializeField] List<ResourceData> _worldResources = new();
        [SerializeField] private InventoryData _startingInventory;
        
        //PLAYER DATA
        [SerializeField] PlayerSO _playerData;
        
        //CIVILIZATIONS DATA
        [SerializeField] List<CivilizationSO> _civilizationSOs = new();
        
        [SerializeField] private CivilizationManager _civilizationManager;
        private Entity.Player.Player _player;
        private CommandInvoker _invoker;
        private IGameTime _gameTime;

        private void Awake()
        {
            base.Awake();
            _invoker = new CommandInvoker();
            
            
        }

        private void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            _gameTime.OnCycleCompleted += UpdateWorld;
            _invoker.OnCommandExecuted += UpdateWorldOnCommand;
            _player = new Entity.Player.Player(_invoker, _playerData, new StorageSystem(_worldResources, _startingInventory));
        }
        

        [ContextMenu("Add Civilization")]
        public void AddCivilizationFromInspector()
        {
            Civilization newCiv = new Civilization(_invoker, _civilizationSOs[0]);
            _civilizationManager.AddCivilization(newCiv);
            _civilizationSOs.RemoveAt(0);
        }
        
        public void AddCivilization(string civ)
        {
            foreach (var civSO in _civilizationSOs)
            {
                if (civSO.civName == civ)
                {
                    Civilization newCiv = new Civilization(_invoker, civSO);
                    _civilizationManager.AddCivilization(newCiv);
                    return;
                }
            }
        }

        private void UpdateWorld(int currentTurn)
        {
            var context = GetWorldContext();
            _civilizationManager.UpdateCivilizations(context);
        }
        private void UpdateWorldOnCommand(ICommand command)
        {
            var context = GetWorldContext();
            _civilizationManager.UpdateCivilizations(context, command);
        }

        private WorldContext GetWorldContext()
        {
            WorldContext context = new WorldContext(_gameTime.CurrentCycle, _player);
            return context;
        }

        
    }
}