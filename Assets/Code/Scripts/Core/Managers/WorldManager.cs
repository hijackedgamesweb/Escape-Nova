using System;
using System.Collections.Generic;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Civilization;
using Code.Scripts.Core.Systems.Civilization.ScriptableObjects;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World;
using Code.Scripts.Patterns.Command;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Player;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class WorldManager : MonoBehaviour
    {
        //STORAGE SYSTEM
        [SerializeField] List<ResourceData> _worldResources = new();
        
        //PLAYER DATA
        
        //CIVILIZATIONS DATA
        [SerializeField] List<CivilizationSO> _civilizationSOs = new();
        
        private Player.Player _player;
        private CivilizationManager _civilizationManager = new();
        private CommandInvoker _invoker;
        private IGameTime _gameTime;

        private void Awake()
        {
            _invoker = new CommandInvoker();
            _player = new Player.Player(_invoker, _worldResources);
        }

        private void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            _gameTime.OnCycleCompleted += UpdateWorld;
        }

        [ContextMenu("Add Civilization")]
        public void AddCivilizationFromInspector()
        {
            Civilization newCiv = new Civilization(_invoker, _civilizationSOs[0]);
            _civilizationManager.AddCivilization(newCiv);
        }
        
        public void AddCivilization(Civilization civ)
        {
            _civilizationManager.AddCivilization(civ);
        }

        private void UpdateWorld(int currentTurn)
        {
            var context = GetWorldContext();
            _civilizationManager.UpdateCivilizations(context);
        }

        private WorldContext GetWorldContext()
        {
            WorldContext context = new WorldContext(_gameTime.CurrentCycle);
            return context;
        }

        
    }
}
