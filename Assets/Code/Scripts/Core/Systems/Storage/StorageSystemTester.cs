using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Core.Systems.Storage
{
    public class StorageSystemTester : BaseUIScreen
    {
        [Header("UI References")]
        public TMP_Text resourcesText;
        public Button addMetalButton;
        public Button addWoodButton;
        public Button addStoneButton;
        public Button consumeMetalButton;
        public Button consumeWoodButton;
        public Button consumeStoneButton;
        public Button exitButton;
        
        [Header("Add Amounts")]
        public int addMetalAmount = 10;
        public int addWoodAmount = 5;
        public int addStoneAmount = 8;
        
        [Header("Consume Amounts")]
        public int consumeMetalAmount = 3;
        public int consumeWoodAmount = 2;
        public int consumeStoneAmount = 4;

        private StorageSystem _storage;

        void Start()
        {
            // Esperamos un frame para que StorageSystem se inicialice
            _storage = ServiceLocator.GetService<StorageSystem>();
            
            // Configurar botones
            addMetalButton.onClick.AddListener(() => AddMetal());
            addWoodButton.onClick.AddListener(() => AddWood());
            addStoneButton.onClick.AddListener(() => AddStone());
            consumeMetalButton.onClick.AddListener(() => ConsumeMetal());
            consumeWoodButton.onClick.AddListener(() => ConsumeWood());
            consumeStoneButton.onClick.AddListener(() => ConsumeStone());
            
            Debug.Log("StorageSystem Tester inicializado!");
        }


        void AddMetal()
        {
            bool success = _storage.AddResource(ResourceType.Metal, addMetalAmount);
            Debug.Log(success ? $"Added {addMetalAmount} Metal" : "Failed to add Metal");
        }

        void AddWood()
        {
            bool success = _storage.AddResource(ResourceType.Sand, addWoodAmount);
            Debug.Log(success ? $"Added {addWoodAmount} Wood" : "Failed to add Wood");
        }

        void AddStone()
        {
            bool success = _storage.AddResource(ResourceType.Stone, addStoneAmount);
            Debug.Log(success ? $"Added {addStoneAmount} Stone" : "Failed to add Stone");
        }

        void ConsumeMetal()
        {
            bool success = _storage.ConsumeResource(ResourceType.Metal, consumeMetalAmount);
            Debug.Log(success ? $"Consumed {consumeMetalAmount} Metal" : "Not enough Metal");
        }

        void ConsumeWood()
        {
            bool success = _storage.ConsumeResource(ResourceType.Sand, consumeWoodAmount);
            Debug.Log(success ? $"Consumed {consumeWoodAmount} Wood" : "Not enough Wood");
        }

        void ConsumeStone()
        {
            bool success = _storage.ConsumeResource(ResourceType.Stone, consumeStoneAmount);
            Debug.Log(success ? $"Consumed {consumeStoneAmount} Stone" : "Not enough Stone");
        }
        
    }
}