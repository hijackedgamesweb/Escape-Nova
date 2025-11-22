using Code.Scripts.Core.Systems.Civilization.AI;
using Code.Scripts.Core.Systems.Diplomacy.AI;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.Interfaces;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.Command;
using UnityEngine;

namespace Code.Scripts.Core.Entity.Civilization
{
    public class Civilization : Entity
    {
        public CivilizationData CivilizationData {get; private set; }
        public CivilizationState CivilizationState  {get; private set; }
        private CommandInvoker _invoker;
        public IAIController AIController { get; private set; }
        
        public Civilization(CommandInvoker invoker, CivilizationSO civilizationSO) : base(invoker, civilizationSO, null)
        {
            _invoker = invoker;
            
            CivilizationData = new CivilizationData(civilizationSO);
            CivilizationState = new CivilizationState(civilizationSO);
            //ItemPreferences = new EntityItemPreferences(civilizationSO.itemPreferences);
            //StorageSystem = new StorageSystem(civilizationSO.startingResources, civilizationSO.startingInventory);
            AIController = AIControllerFactory.CreateAIController(civilizationSO.aiController, this, invoker);
        }
    }
}