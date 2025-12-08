using System;
using System.Collections.Generic;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Patterns.Singleton;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Systems.Crafting;
using Code.Scripts.Core.Systems.Research;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Astrarium
{
    public class AstrariumManager : InGameSingleton<AstrariumManager>, ISaveable
    {
        [Header("Data")]
        [SerializeField] private AstrariumDatabaseSO _database;

        private Dictionary<string, DiscoveryStatus> _discoveryState = new Dictionary<string, DiscoveryStatus>();

        public event Action<string, DiscoveryStatus> OnEntryUpdated;
        public event Action OnDatabaseReloaded;

        protected override void Awake()
        {
            base.Awake();
            if (_database != null) _database.Initialize();
        }

        private void Start()
        {
            SubscribeToEvents();
            UnlockInitialResources();
            CheckConstellationsUnlock();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            ResearchEvents.OnResearchCompleted += HandleResearchCompleted;
            ConstructionEvents.OnPlanetAdded += HandlePlanetAdded; 
            ConstructionEvents.OnConstructibleCreated += HandleConstructibleCreated; 
            
            var craftingSystem = ServiceLocator.GetService<CraftingSystem>();
            if (craftingSystem != null) craftingSystem.OnItemCrafted += HandleItemCrafted;

            DiplomacyEvents.OnCivilizationMet += HandleCivilizationMet;

            SystemEvents.OnConstellationsUnlocked += UnlockAllConstellations;
        }

        private void UnsubscribeFromEvents()
        {
            ResearchEvents.OnResearchCompleted -= HandleResearchCompleted;
            ConstructionEvents.OnPlanetAdded -= HandlePlanetAdded;
            ConstructionEvents.OnConstructibleCreated -= HandleConstructibleCreated;
            
            var craftingSystem = ServiceLocator.GetService<CraftingSystem>();
            if (craftingSystem != null) craftingSystem.OnItemCrafted -= HandleItemCrafted;
            
            SystemEvents.OnConstellationsUnlocked -= UnlockAllConstellations;
        }
        
        private void HandleResearchCompleted(string researchId)
        {
            if (string.IsNullOrEmpty(researchId)) return;

            string cleanId = researchId.ToLower().Trim();

            if (TryUnlock(cleanId)) return;
            
            if (TryUnlock($"planet_{cleanId}")) return;
            if (TryUnlock($"sat_{cleanId}")) return;
            if (TryUnlock($"resource_{cleanId}")) return;
            if (TryUnlock($"species_{cleanId}")) return;
            if (TryUnlock($"stars_{cleanId}")) return;
            if (TryUnlock($"constellation_{cleanId}")) return;

        }
        private bool TryUnlock(string testId)
        {
            var entry = _database.GetEntry(testId);
            if (entry != null)
            {
                if (entry.GetCategory() == AstrariumCategory.Special)
                {
                    UnlockEntry(testId, DiscoveryStatus.Analyzed);
                }
                else
                {
                    UnlockEntry(testId, DiscoveryStatus.Discovered);
                }
                return true; // Éxito
            }
            return false; // Sigue buscando
        }
        private void HandleResearchCompleted(ResearchNode node) => HandleResearchCompleted(node?.researchId);

        private void HandlePlanetAdded(Code.Scripts.Core.World.ConstructableEntities.Planet planet)
        {
            if (planet.PlanetData is IAstrariumEntry entry)
                UnlockEntry(entry.GetAstrariumID(), DiscoveryStatus.Analyzed);
        }

        private void HandleConstructibleCreated(ScriptableObject data)
        {
            if (data is IAstrariumEntry entry)
                UnlockEntry(entry.GetAstrariumID(), DiscoveryStatus.Analyzed);
        }
        private void HandleCivilizationMet(CivilizationSO civData)
        {
            if (civData is IAstrariumEntry entry)
                UnlockEntry(entry.GetAstrariumID(), DiscoveryStatus.Analyzed);
        }

        private void HandleItemCrafted(string recipeId, int amount)
        {
            var craftingSystem = ServiceLocator.GetService<CraftingSystem>();
            var recipe = craftingSystem.GetRecipe(recipeId);
            if (recipe != null)
            {
                var itemData = craftingSystem.GetItemData(recipe.output.itemName);
                if (itemData is ScriptableObject so && so is IAstrariumEntry entry)
                    UnlockEntry(entry.GetAstrariumID(), DiscoveryStatus.Analyzed);
            }
        }
        
        public void UnlockEntry(string id, DiscoveryStatus targetStatus)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (_database.GetEntry(id) == null) return;

            DiscoveryStatus currentStatus = _discoveryState.GetValueOrDefault(id, DiscoveryStatus.Unknown);

            // Solo actualizamos si mejoramos el estado
            if (targetStatus > currentStatus)
            {
                _discoveryState[id] = targetStatus;
                OnEntryUpdated?.Invoke(id, targetStatus);
            }
        }

        public IAstrariumEntry GetEntryData(string id) => _database.GetEntry(id);

        public List<IAstrariumEntry> GetUnlockedEntriesByCategory(AstrariumCategory category)
        {
            List<IAstrariumEntry> result = new List<IAstrariumEntry>();
            var all = _database.GetAllEntries();

            foreach (var entry in all)
            {
                if (entry.GetCategory() == category)
                {
                    if (_discoveryState.TryGetValue(entry.GetAstrariumID(), out var status) && status != DiscoveryStatus.Unknown)
                    {
                        result.Add(entry);
                    }
                }
            }
            return result;
        }

        public DiscoveryStatus GetStatus(string id) => _discoveryState.GetValueOrDefault(id, DiscoveryStatus.Unknown);
        
        private void CheckConstellationsUnlock()
        {
            if (SystemEvents.IsConstellationsUnlocked) UnlockAllConstellations();
        }
        private void UnlockAllConstellations()
        {
            foreach (var entry in _database.GetAllEntries())
                if (entry.GetCategory() == AstrariumCategory.Constellation)
                    UnlockEntry(entry.GetAstrariumID(), DiscoveryStatus.Analyzed);
        }
        private void UnlockInitialResources()
        {
            if (_database == null) return;
            foreach (var entry in _database.GetAllEntries())
                if (entry.GetCategory() == AstrariumCategory.Resource)
                    UnlockEntry(entry.GetAstrariumID(), DiscoveryStatus.Analyzed);
        }
        
        public string GetSaveId() => "AstrariumManager";
        public JToken CaptureState() {
            JObject state = new JObject();
            JObject entries = new JObject();
            foreach (var kvp in _discoveryState) entries[kvp.Key] = (int)kvp.Value;
            state["entries"] = entries;
            return state;
        }
        public void RestoreState(JToken state) {
            _discoveryState.Clear();
            JObject obj = state as JObject;
            if (obj != null && obj["entries"] != null) {
                JObject entries = (JObject)obj["entries"];
                foreach (var prop in entries.Properties())
                    _discoveryState[prop.Name] = (DiscoveryStatus)(int)prop.Value;
            }
            OnDatabaseReloaded?.Invoke();
            UnlockInitialResources();
            CheckConstellationsUnlock();
        }
    }
}