using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.Systems.Time;
using Code.Scripts.Patterns.ServiceLocator;
using ResourceType = Code.Scripts.Core.Systems.Resources.ResourceType;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Skills
{
    public class SkillTreeManager : MonoBehaviour
    {
        [Header("Skill Point Settings")]
        [SerializeField] private int initialSkillPoints = 3;
        [SerializeField] private int skillPointsPerCycle = 1;
        [SerializeField] private int cyclesPerPoint = 10;

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
        }

        private void OnDestroy()
        {
            if (gameTime != null)
            {
                gameTime.OnCycleCompleted -= OnCycleCompleted;
            }
            ServiceLocator.UnregisterService<SkillTreeManager>();
        }

        private void Start()
        {
            StartCoroutine(InitializeWhenReady());
        }

        private void InitializeStorageModifiers()
        {
            var allResources = Enum.GetValues(typeof(ResourceType));
            foreach (ResourceType resource in allResources)
            {
                storageModifiers[resource] = new List<StorageModifier>();
            }
        }

        private IEnumerator InitializeWhenReady()
        {
            yield return WaitForService<IGameTime>((service) => gameTime = service, "IGameTime");
            yield return WaitForService<StorageSystem>((service) => storageSystem = service, "StorageSystem", false);

            InitializeNodeStates();

            if (gameTime != null)
            {
                gameTime.OnCycleCompleted += OnCycleCompleted;
                lastCycleCounted = gameTime.CurrentCycle;
            }

            AddSkillPoints(initialSkillPoints);
            isInitialized = true;

            Debug.Log("SkillTreeManager: Initialization completed successfully");
        }

        private IEnumerator WaitForService<T>(Action<T> setService, string serviceName, bool isRequired = true)
        {
            int attempts = 0;
            int maxAttempts = 50;

            T service = default(T);

            while (service == null && attempts < maxAttempts)
            {
                try
                {
                    service = ServiceLocator.GetService<T>();
                }
                catch (Exception e)
                {
                    Debug.Log("SkillTreeManager: Attempt " + (attempts + 1) + " - " + serviceName + " not ready: " + e.Message);
                }

                if (service == null)
                {
                    attempts++;
                    yield return new WaitForSeconds(0.1f);
                }
            }

            if (service != null)
            {
                setService(service);
                Debug.Log($"SkillTreeManager: Successfully acquired {serviceName}");
            }
            else if (isRequired)
            {
                Debug.LogError($"SkillTreeManager: Failed to get required service {serviceName} after {maxAttempts} attempts");
            }
        }

        private void OnCycleCompleted(int currentCycle)
        {
            if (!isInitialized) return;

            Debug.Log("SkillTreeManager: Cycle completed - Current: " + currentCycle + ", Last counted: " + lastCycleCounted);

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
                    Debug.Log("SkillTreeManager: Gained " + pointsToAdd + " skill points after " + cyclesPassed + " cycles (from cycle " + lastCycleCounted + " to " + currentCycle + ")");
                }
                else
                {
                    int cyclesUntilNextPoint = cyclesPerPoint - (currentCycle % cyclesPerPoint);
                    Debug.Log("SkillTreeManager: " + cyclesUntilNextPoint + " cycles until next point");
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
            Debug.Log("SkillTreeManager: Skill points updated: " + availableSkillPoints);
        }

        public bool CanPurchaseSkill(SkillNodeData nodeData)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("SkillTreeManager not initialized");
                return false;
            }
            if (nodeData == null)
            {
                Debug.LogWarning("NodeData is null");
                return false;
            }
            if (!nodeStates.ContainsKey(nodeData.name))
            {
                Debug.LogWarning($"Node state not found for: {nodeData.name}");
                return false;
            }

            var state = nodeStates[nodeData.name];
            bool canPurchase = state.isUnlocked && !state.isPurchased && availableSkillPoints >= nodeData.skillPointCost;

            Debug.Log($"CanPurchase {nodeData.nodeName}: Unlocked={state.isUnlocked}, Purchased={state.isPurchased}, Points={availableSkillPoints}, Cost={nodeData.skillPointCost}, Result={canPurchase}");

            return canPurchase;
        }

        public bool PurchaseSkill(SkillNodeData nodeData)
        {
            if (!CanPurchaseSkill(nodeData))
            {
                Debug.LogWarning($"Cannot purchase node: {nodeData.nodeName}");
                return false;
            }

            availableSkillPoints -= nodeData.skillPointCost;
            nodeStates[nodeData.name].isPurchased = true;

            ApplySkillImprovements(nodeData);
            UnlockNextNodes();

            OnSkillPurchased?.Invoke(nodeData);
            OnSkillPointsChanged?.Invoke(availableSkillPoints);

            Debug.Log($"Successfully purchased node: {nodeData.nodeName}. Remaining points: {availableSkillPoints}");

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
                    Debug.Log($"Applied improvement from node: {nodeData.nodeName}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error applying improvement for node {nodeData.nodeName}: {e.Message}");
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
                        Debug.Log($"Node {node.nodeName} unlocked");
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
            Debug.Log($"=== SKILL TREE MANAGER DEBUG ===");
            Debug.Log($"Initialized: {isInitialized}");
            Debug.Log($"Available Skill Points: {availableSkillPoints}");
            Debug.Log($"Total Nodes: {nodeStates.Count}");
            Debug.Log($"Constellations: {constellations?.Count ?? 0}");

            if (constellations != null)
            {
                foreach (var constellation in constellations)
                {
                    if (constellation?.nodes == null) continue;

                    Debug.Log($"Constellation: {constellation.constellationName} - Nodes: {constellation.nodes.Count}");

                    foreach (var node in constellation.nodes)
                    {
                        if (node == null) continue;

                        if (nodeStates.ContainsKey(node.name))
                        {
                            var state = nodeStates[node.name];
                            Debug.Log($"  Node: {node.nodeName} - Purchased: {state.isPurchased} - Unlocked: {state.isUnlocked}");
                        }
                        else
                        {
                            Debug.Log($"  Node: {node.nodeName} - NO STATE FOUND");
                        }
                    }
                }
            }
        }
    }
}