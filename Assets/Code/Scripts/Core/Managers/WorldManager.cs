using System;
using System.Collections.Generic;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Entity.Player;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.SaveLoad;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.Systems.Civilization;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.Command.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Patterns.Singleton;
using Code.Scripts.Player;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class WorldManager : InGameSingleton<WorldManager>, ISaveable
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
        public Entity.Player.Player Player => _player;
        private CommandInvoker _invoker;
        private IGameTime _gameTime;
        protected override void Awake()
        {
            base.Awake();
            _invoker = new CommandInvoker();
            
            StorageSystem playerStorage = new StorageSystem(_worldResources, _startingInventory);
            ServiceLocator.RegisterService<StorageSystem>(playerStorage);
            
            _player = new Entity.Player.Player(_invoker, _playerData, playerStorage);
        }

        private async void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            
            if (_gameTime != null)
            {
                _gameTime.OnCycleCompleted += UpdateWorld;
            }
            
            _invoker.OnCommandExecuted += UpdateWorldOnCommand;
            ConstructionEvents.OnPlanetAdded += OnPlanetConstructed;

            if (SaveManager.Instance != null && SaveManager.Instance.SlotExists())
            {
                await SaveManager.Instance.LoadSlotAsync();
            }
        }

        private void OnPlanetConstructed(Planet obj)
        {
            foreach (var civ in _civilizationSOs)
            {
                if (civ.preferredPlanet != null && civ.preferredPlanet.constructibleName == obj.Name)
                {
                    AddCivilization(civ.civName);
                    break;
                }
            }
        }

        [ContextMenu("Add Civilization")]
        public void AddCivilizationFromInspector()
        {
            if (_civilizationSOs.Count > 0)
            {
                Civilization newCiv = new Civilization(_invoker, _civilizationSOs[0]);
                _civilizationManager.AddCivilization(newCiv);
                _civilizationSOs.RemoveAt(0);
            }
        }
        
        public void AddCivilization(string civ)
        {
            foreach (var civSO in _civilizationSOs)
            {
                if (civSO.civName == civ)
                {
                    Civilization newCiv = new Civilization(_invoker, civSO);
                    _civilizationManager.AddCivilization(newCiv);
                    _civilizationSOs.Remove(civSO);
                    DiplomacyEvents.OnCivilizationMet?.Invoke(civSO);
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
            int cycle = _gameTime != null ? _gameTime.CurrentCycle : 0;
            WorldContext context = new WorldContext(cycle, _player);
            return context;
        }

        public void AddAllCivilizations()
        {
            foreach (var civSO in _civilizationSOs)
            {
                Civilization newCiv = new Civilization(_invoker, civSO);
                _civilizationManager.AddCivilization(newCiv);
            }
            _civilizationSOs.Clear();
        }

        public string GetSaveId()
        {
            return "WorldContext";
        }

        public JToken CaptureState()
        {
            JObject state = new JObject();
            state["Player"] = JToken.FromObject(_player.CaptureState());
            state["Civilizations"] = JToken.FromObject(_civilizationManager.CaptureState());
            return state;
        }

        public void RestoreState(JToken state)
        {
            JObject obj = state as JObject;
            if (obj != null)
            {
                if(obj["Player"] != null) _player.RestoreState(obj["Player"]);
                if(obj["Civilizations"] != null) _civilizationManager.RestoreState(obj["Civilizations"]);
            }
        }
    }
}