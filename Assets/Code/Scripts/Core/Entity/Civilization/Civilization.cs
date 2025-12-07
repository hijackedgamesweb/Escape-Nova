using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.Systems.Civilization.AI;
using Code.Scripts.Core.Systems.Diplomacy.AI;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.Interfaces;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.Command;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Code.Scripts.Core.Entity.Civilization
{
    public class Civilization : Entity, ISaveable
    {
        public CivilizationData CivilizationData {get; private set; }
        public CivilizationState CivilizationState  {get; private set; }
        private CommandInvoker _invoker;
        public IAIController AIController { get; private set; }
        public AIType AIControllerType;
        
        public Civilization(CommandInvoker invoker, CivilizationSO civilizationSO) : base(invoker, civilizationSO, null)
        {
            _invoker = invoker;
            
            CivilizationData = new CivilizationData(civilizationSO);
            CivilizationState = new CivilizationState(civilizationSO);
            ItemPreferences = new EntityItemPreferences(civilizationSO.itemPreferences);
            StorageSystem = new StorageSystem(civilizationSO.startingResources, civilizationSO.startingInventory);
            StorageSystem.SetResourceAmounts(civilizationSO.startingResources, civilizationSO.startingResourceAmounts);
            AIController = AIControllerFactory.CreateAIController(civilizationSO.aiController, this, invoker);
            AIControllerType = civilizationSO.aiController;
        }

        public Civilization() : base()
        {
            CivilizationData = new CivilizationData();
            CivilizationState = new CivilizationState();
            StorageSystem = new StorageSystem();
        }
        
        public void SetCivilizationData(CivilizationSO civilizationSO)
        {
            CivilizationData = new CivilizationData(civilizationSO);
            CivilizationState = new CivilizationState(civilizationSO);
            ItemPreferences = new EntityItemPreferences(civilizationSO.itemPreferences);
            StorageSystem = new StorageSystem(civilizationSO.startingResources, civilizationSO.startingInventory);
            StorageSystem.SetResourceAmounts(civilizationSO.startingResources, civilizationSO.startingResourceAmounts);
            AIController = AIControllerFactory.CreateAIController(civilizationSO.aiController, this, _invoker);
            AIControllerType = civilizationSO.aiController;
        }

        public string GetSaveId()
        {
            return $"Civilization_{CivilizationData.Name}";
        }

        public JToken CaptureState()
        {
            JObject obj = new JObject
            {
                ["CivilizationData"] = CivilizationData.CaptureState(),
                ["CivilizationState"] = CivilizationState.CaptureState(),
                ["StorageSystem"] = StorageSystem.CaptureState(),
                ["AIController"] = (int)AIControllerType
            };
            return obj;
        }

        public void RestoreState(JToken state)
        {
            JObject obj = state as JObject;
            CivilizationData.RestoreState(obj["CivilizationData"]);
            CivilizationState.RestoreState(obj["CivilizationState"]);
            StorageSystem.RestoreState(obj["StorageSystem"]);
            AIControllerType = (AIType)obj["AIController"].ToObject<int>();
            AIController = AIControllerFactory.CreateAIController(AIControllerType, this, _invoker);
        }
    }
}