using System;
using System.Collections.Generic;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.Systems.Time;
using Code.Scripts.Patterns.ServiceLocator;
using Newtonsoft.Json.Linq;
using ResourceType = Code.Scripts.Core.Systems.Resources.ResourceType;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Skills
{
    public class SkillTreeManager : MonoBehaviour, ISaveable
    {
        [Header("Skill Point Settings")]
        [SerializeField] private int initialSkillPoints = 2;
        [SerializeField] private int skillPointsPerCycle = 1;
        [SerializeField] private int cyclesPerPoint = 150;

        [Header("Constellations")]
        [SerializeField] private List<SkillConstellation> constellations = new List<SkillConstellation>();

        private Dictionary<string, SkillNodeState> nodeStates = new Dictionary<string, SkillNodeState>();
        private int availableSkillPoints = 0;
        private IGameTime gameTime;
        private StorageSystem storageSystem;
        private bool isInitialized = false;
        private int lastCycleCounted = 0;

        private Dictionary<ResourceType, List<StorageModifier>> storageModifiers = new Dictionary<ResourceType, List<StorageModifier>>();

        public event Action<SkillNodeData> OnSkillPurchased;
        public event Action<int> OnSkillPointsChanged;

        // Propiedad pública para verificar si está inicializado
        public bool IsInitialized => isInitialized;

        [System.Serializable]
        private class SkillNodeState
        {
            public bool isPurchased;
            public bool isUnlocked;
        }

        [System.Serializable]
        private class StorageModifier
        {
            public string sourceNode;
            public float increaseAmount;
        }

        private void Awake()
        {
            ServiceLocator.RegisterService<SkillTreeManager>(this);
            InitializeStorageModifiers();

            // Inicializar inmediatamente en Awake en lugar de Start
            InitializeImmediate();
        }

        private void OnDestroy()
        {
            if (gameTime != null)
            {
                gameTime.OnCycleCompleted -= OnCycleCompleted;
            }
            ServiceLocator.UnregisterService<SkillTreeManager>();
        }

        private void InitializeImmediate()
        {
            ////Debug.Log("SkillTreeManager: Starting immediate initialization...");

            // Intentar obtener servicios inmediatamente
            TryGetServices();

            if (AreServicesReady())
            {
                CompleteInitialization();
            }
            else
            {
                ////Debug.Log("SkillTreeManager: Services not ready, starting rapid retry...");
                InvokeRepeating(nameof(TryCompleteInitialization), 0.05f, 0.05f);
            }
        }

        private void TryCompleteInitialization()
        {
            if (isInitialized)
            {
                CancelInvoke(nameof(TryCompleteInitialization));
                return;
            }

            TryGetServices();

            if (AreServicesReady())
            {
                CancelInvoke(nameof(TryCompleteInitialization));
                CompleteInitialization();
            }
        }

        private void TryGetServices()
        {
            // Intentar obtener IGameTime
            if (gameTime == null)
            {
                try
                {
                    gameTime = ServiceLocator.GetService<IGameTime>();
                    //if (gameTime != null)
                        //Debug.Log("SkillTreeManager: Successfully acquired IGameTime");
                }
                catch (Exception e)
                {
                    //Debug.Log($"SkillTreeManager: IGameTime not available: {e.Message}");
                }
            }

            // Intentar obtener StorageSystem (no crítico)
            if (storageSystem == null)
            {
                try
                {
                    storageSystem = WorldManager.Instance.Player.StorageSystem;
                    //if (storageSystem != null)
                        //Debug.Log("SkillTreeManager: Successfully acquired StorageSystem");
                }
                catch (Exception e)
                {
                    // StorageSystem no es crítico, solo log si es necesario
                }
            }
        }

        private bool AreServicesReady()
        {
            return gameTime != null; // Solo IGameTime es crítico
        }

        private void CompleteInitialization()
        {
            InitializeNodeStates();

            if (gameTime != null)
            {
                gameTime.OnCycleCompleted += OnCycleCompleted;
                lastCycleCounted = gameTime.CurrentCycle;
            }

            AddSkillPoints(initialSkillPoints);
            isInitialized = true;

            //Debug.Log("SkillTreeManager: Initialization completed successfully");
        }

        private void InitializeStorageModifiers()
        {
            var allResources = Enum.GetValues(typeof(ResourceType));
            foreach (ResourceType resource in allResources)
            {
                storageModifiers[resource] = new List<StorageModifier>();
            }
        }

        private void OnCycleCompleted(int currentCycle)
        {
            if (!isInitialized) return;

            //Debug.Log("SkillTreeManager: Cycle completed - Current: " + currentCycle + ", Last counted: " + lastCycleCounted);

            int cyclesPassed = currentCycle - lastCycleCounted;

            if (cyclesPassed > 0)
            {
                int pointsToAdd = 0;
                for (int i = 0; i < cyclesPassed; i++)
                {
                    if ((lastCycleCounted + i + 1) % cyclesPerPoint == 0)
                    {
                        pointsToAdd += skillPointsPerCycle;
                    }
                }

                if (pointsToAdd > 0)
                {
                    AddSkillPoints(pointsToAdd);
                    //Debug.Log("SkillTreeManager: Gained " + pointsToAdd + " skill points after " + cyclesPassed + " cycles (from cycle " + lastCycleCounted + " to " + currentCycle + ")");
                }
                else
                {
                    int cyclesUntilNextPoint = cyclesPerPoint - (currentCycle % cyclesPerPoint);
                    //Debug.Log("SkillTreeManager: " + cyclesUntilNextPoint + " cycles until next point");
                }

                lastCycleCounted = currentCycle;
            }
        }

        private void InitializeNodeStates()
        {
            if (constellations == null) return;

            nodeStates.Clear();

            // Primera pasada: crear todos los estados
            foreach (var constellation in constellations)
            {
                if (constellation?.nodes == null) continue;

                foreach (var node in constellation.nodes)
                {
                    if (node == null) continue;

                    nodeStates[node.name] = new SkillNodeState
                    {
                        isPurchased = false,
                        isUnlocked = ArePrerequisitesMet(node)
                    };
                }
            }
        }

        private bool ArePrerequisitesMet(SkillNodeData node)
        {
            if (node.prerequisiteNodes == null || node.prerequisiteNodes.Count == 0)
                return true; // Sin prerrequisitos = siempre desbloqueado

            foreach (var prereq in node.prerequisiteNodes)
            {
                if (prereq == null) continue;

                if (!nodeStates.ContainsKey(prereq.name) || !nodeStates[prereq.name].isPurchased)
                    return false;
            }

            return true;
        }

        public void AddSkillPoints(int points)
        {
            if (!isInitialized) return;

            availableSkillPoints += points;
            OnSkillPointsChanged?.Invoke(availableSkillPoints);
            //Debug.Log("SkillTreeManager: Skill points updated: " + availableSkillPoints);
        }

        public bool CanPurchaseSkill(SkillNodeData nodeData)
        {
            if (!isInitialized)
            {
                //Debug.LogWarning("SkillTreeManager not initialized");
                return false;
            }
            if (nodeData == null)
            {
                //Debug.LogWarning("NodeData is null");
                return false;
            }
            if (!nodeStates.ContainsKey(nodeData.name))
            {
                //Debug.LogWarning($"Node state not found for: {nodeData.name}");
                return false;
            }

            var state = nodeStates[nodeData.name];
            bool canPurchase = state.isUnlocked && !state.isPurchased && availableSkillPoints >= nodeData.skillPointCost;

            //Debug.Log($"CanPurchase {nodeData.nodeName}: Unlocked={state.isUnlocked}, Purchased={state.isPurchased}, Points={availableSkillPoints}, Cost={nodeData.skillPointCost}, Result={canPurchase}");

            return canPurchase;
        }

        public bool PurchaseSkill(SkillNodeData nodeData)
        {
            if (!CanPurchaseSkill(nodeData))
            {
                //Debug.LogWarning($"Cannot purchase node: {nodeData.nodeName}");
                return false;
            }

            availableSkillPoints -= nodeData.skillPointCost;
            nodeStates[nodeData.name].isPurchased = true;

            ApplySkillImprovements(nodeData);
            UnlockNextNodes();

            OnSkillPurchased?.Invoke(nodeData);
            OnSkillPointsChanged?.Invoke(availableSkillPoints);

            //Debug.Log($"Successfully purchased node: {nodeData.nodeName}. Remaining points: {availableSkillPoints}");

            return true;
        }

        private void ApplySkillImprovements(SkillNodeData nodeData)
        {
            if (nodeData.improvements == null) return;

            foreach (var improvement in nodeData.improvements)
            {
                try
                {
                    improvement.ApplyImprovement();
                    //Debug.Log($"Applied improvement from node: {nodeData.nodeName}");
                }
                catch (System.Exception e)
                {
                    //Debug.LogError($"Error applying improvement for node {nodeData.nodeName}: {e.Message}");
                }
            }
        }

        private void UnlockNextNodes()
        {
            bool anyUnlocked = false;

            foreach (var constellation in constellations)
            {
                if (constellation?.nodes == null) continue;

                foreach (var node in constellation.nodes)
                {
                    if (node == null || nodeStates.ContainsKey(node.name) == false)
                        continue;

                    var state = nodeStates[node.name];

                    // Si ya está comprado o desbloqueado, saltar
                    if (state.isPurchased || state.isUnlocked)
                        continue;

                    // Verificar si ahora cumple los prerrequisitos
                    if (ArePrerequisitesMet(node))
                    {
                        state.isUnlocked = true;
                        anyUnlocked = true;
                        //Debug.Log($"Node {node.nodeName} unlocked");
                    }
                }
            }

            if (anyUnlocked)
            {
                RefreshAllNodeUI();
            }
        }

        private void RefreshAllNodeUI()
        {
            var skillTreeUIs = FindObjectsOfType<SkillTreeUI>();
            foreach (var skillTreeUI in skillTreeUIs)
            {
                if (skillTreeUI != null && skillTreeUI.isActiveAndEnabled)
                {
                    skillTreeUI.RefreshUI();
                }
            }
        }

        public bool IsSkillPurchased(SkillNodeData nodeData)
        {
            return isInitialized && nodeData != null && nodeStates.ContainsKey(nodeData.name) && nodeStates[nodeData.name].isPurchased;
        }

        public bool IsSkillUnlocked(SkillNodeData nodeData)
        {
            return isInitialized && nodeData != null && nodeStates.ContainsKey(nodeData.name) && nodeStates[nodeData.name].isUnlocked;
        }

        public int GetAvailableSkillPoints() => isInitialized ? availableSkillPoints : 0;

        public List<SkillConstellation> GetConstellations() => constellations;

        public float GetTotalStorageIncrease(ResourceType resourceType)
        {
            if (!storageModifiers.ContainsKey(resourceType)) return 0f;

            float total = 0f;
            foreach (var modifier in storageModifiers[resourceType])
            {
                total += modifier.increaseAmount;
            }
            return total;
        }

        [ContextMenu("Debug Node States")]
        public void DebugNodeStates()
        {
            //Debug.Log($"=== SKILL TREE MANAGER DEBUG ===");
            //Debug.Log($"Initialized: {isInitialized}");
            //Debug.Log($"Available Skill Points: {availableSkillPoints}");
            //Debug.Log($"Total Nodes: {nodeStates.Count}");
            //Debug.Log($"Constellations: {constellations?.Count ?? 0}");

            if (constellations != null)
            {
                foreach (var constellation in constellations)
                {
                    if (constellation?.nodes == null) continue;

                    //Debug.Log($"Constellation: {constellation.constellationName} - Nodes: {constellation.nodes.Count}");

                    foreach (var node in constellation.nodes)
                    {
                        if (node == null) continue;

                        if (nodeStates.ContainsKey(node.name))
                        {
                            var state = nodeStates[node.name];
                            //Debug.Log($"  Node: {node.nodeName} - Purchased: {state.isPurchased} - Unlocked: {state.isUnlocked}");
                        }
                        else
                        {
                            //Debug.Log($"  Node: {node.nodeName} - NO STATE FOUND");
                        }
                    }
                }
            }
        }

        public string GetSaveId()
        {
            return "SkillTreeManager";
        }

        public JToken CaptureState()
        {
            JObject state = new JObject
            {
                ["availableSkillPoints"] = availableSkillPoints,
                ["nodeStates"] = new JObject()
            };

            JObject nodeStatesObj = (JObject)state["nodeStates"];

            foreach (var kvp in nodeStates)
            {
                JObject nodeStateObj = new JObject
                {
                    ["isPurchased"] = kvp.Value.isPurchased,
                    ["isUnlocked"] = kvp.Value.isUnlocked
                };
                nodeStatesObj[kvp.Key] = nodeStateObj;
            }

            return state;
        }

        public void RestoreState(JToken state)
        {
            availableSkillPoints = state["availableSkillPoints"]?.ToObject<int>() ?? 0;

            JObject nodeStatesObj = state["nodeStates"] as JObject;
            if (nodeStatesObj != null)
            {
                foreach (var kvp in nodeStatesObj)
                {
                    string nodeName = kvp.Key;
                    JObject nodeStateObj = kvp.Value as JObject;

                    if (nodeStateObj != null)
                    {
                        bool isPurchased = nodeStateObj["isPurchased"]?.ToObject<bool>() ?? false;
                        bool isUnlocked = nodeStateObj["isUnlocked"]?.ToObject<bool>() ?? false;

                        if (nodeStates.ContainsKey(nodeName))
                        {
                            nodeStates[nodeName].isPurchased = isPurchased;
                            nodeStates[nodeName].isUnlocked = isUnlocked;
                        }
                        else
                        {
                            nodeStates[nodeName] = new SkillNodeState
                            {
                                isPurchased = isPurchased,
                                isUnlocked = isUnlocked
                            };
                        }
                    }
                }
            }

            OnSkillPointsChanged?.Invoke(availableSkillPoints);
            RefreshAllNodeUI();
        }
    }
}