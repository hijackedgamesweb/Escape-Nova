using System;
using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.Systems.Time;
using Code.Scripts.Patterns.ServiceLocator;
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

        public event Action<SkillNodeData> OnSkillPurchased;
        public event Action<int> OnSkillPointsChanged;

        [System.Serializable]
        private class SkillNodeState
        {
            public bool isPurchased;
            public bool isUnlocked;
        }

        private void Awake()
        {
            ServiceLocator.RegisterService<SkillTreeManager>(this);
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

        private IEnumerator InitializeWhenReady()
        {
            Debug.Log("SkillTreeManager: Starting initialization...");

            // Esperar a que GameTime esté disponible
            yield return WaitForService<IGameTime>((service) => gameTime = service, "IGameTime");

            // Esperar a que StorageSystem esté disponible (opcional)
            yield return WaitForService<StorageSystem>((service) => storageSystem = service, "StorageSystem", false);

            InitializeNodeStates();

            // Suscribirse al evento de ciclos completados
            if (gameTime != null)
            {
                gameTime.OnCycleCompleted += OnCycleCompleted;
                lastCycleCounted = gameTime.CurrentCycle;
                Debug.Log($"SkillTreeManager: Subscribed to cycle events. Current cycle: {lastCycleCounted}");
            }

            // Puntos iniciales
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
                    Debug.Log($"SkillTreeManager: Attempt {attempts + 1} - {serviceName} not ready: {e.Message}");
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
                Debug.Log($"SkillTreeManager: {serviceName} service acquired");
            }
            else if (isRequired)
            {
                Debug.LogError($"SkillTreeManager: Failed to get {serviceName} service after {maxAttempts} attempts");
            }
            else
            {
                Debug.LogWarning($"SkillTreeManager: {serviceName} service not available, but continuing without it");
            }
        }

        private void OnCycleCompleted(int currentCycle)
        {
            if (!isInitialized) return;

            Debug.Log($"SkillTreeManager: Cycle completed - Current: {currentCycle}, Last counted: {lastCycleCounted}");

            // Calcular cuántos ciclos han pasado desde la última vez
            int cyclesPassed = currentCycle - lastCycleCounted;

            if (cyclesPassed > 0)
            {
                // Calcular cuántos puntos ganar
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
                    Debug.Log($"SkillTreeManager: Gained {pointsToAdd} skill points after {cyclesPassed} cycles (from cycle {lastCycleCounted} to {currentCycle})");
                }
                else
                {
                    int cyclesUntilNextPoint = cyclesPerPoint - (currentCycle % cyclesPerPoint);
                    Debug.Log($"SkillTreeManager: {cyclesUntilNextPoint} cycles until next point");
                }

                lastCycleCounted = currentCycle;
            }
        }

        private void InitializeNodeStates()
        {
            if (constellations == null)
            {
                Debug.LogError("SkillTreeManager: Constellations list is null!");
                return;
            }

            Debug.Log($"SkillTreeManager: Initializing {constellations.Count} constellations");

            foreach (var constellation in constellations)
            {
                if (constellation == null)
                {
                    Debug.LogError("SkillTreeManager: Found null constellation in list!");
                    continue;
                }

                if (constellation.nodes == null)
                {
                    Debug.LogError($"SkillTreeManager: Constellation {constellation.constellationName} has null nodes list!");
                    continue;
                }

                foreach (var node in constellation.nodes)
                {
                    if (node == null)
                    {
                        Debug.LogError("SkillTreeManager: Found null node in constellation!");
                        continue;
                    }

                    if (!nodeStates.ContainsKey(node.name))
                    {
                        bool startsUnlocked = node.prerequisiteNodes == null || node.prerequisiteNodes.Count == 0;
                        nodeStates[node.name] = new SkillNodeState
                        {
                            isPurchased = false,
                            isUnlocked = startsUnlocked
                        };
                    }
                }
            }
        }

        public void AddSkillPoints(int points)
        {
            if (!isInitialized) return;

            availableSkillPoints += points;
            OnSkillPointsChanged?.Invoke(availableSkillPoints);
            Debug.Log($"SkillTreeManager: Skill points updated: {availableSkillPoints}");
        }

        public bool CanPurchaseSkill(SkillNodeData nodeData)
        {
            if (!isInitialized) return false;
            if (nodeData == null) return false;
            if (!nodeStates.ContainsKey(nodeData.name)) return false;

            var state = nodeStates[nodeData.name];
            return state.isUnlocked && !state.isPurchased && availableSkillPoints >= nodeData.skillPointCost;
        }

        public bool PurchaseSkill(SkillNodeData nodeData)
        {
            if (!CanPurchaseSkill(nodeData)) return false;

            availableSkillPoints -= nodeData.skillPointCost;
            nodeStates[nodeData.name].isPurchased = true;

            ApplySkillImprovements(nodeData);
            UnlockNextNodes(nodeData);

            OnSkillPurchased?.Invoke(nodeData);
            OnSkillPointsChanged?.Invoke(availableSkillPoints);

            return true;
        }

        private void ApplySkillImprovements(SkillNodeData nodeData)
        {
            foreach (var improvement in nodeData.improvements)
            {
                Debug.Log($"Applied improvement: {improvement.improvementType} - {improvement.value}");
            }
        }

        private void UnlockNextNodes(SkillNodeData purchasedNode)
        {
            foreach (var constellation in constellations)
            {
                foreach (var node in constellation.nodes)
                {
                    if (node.prerequisiteNodes.Contains(purchasedNode))
                    {
                        if (nodeStates.ContainsKey(node.name))
                        {
                            nodeStates[node.name].isUnlocked = true;
                        }
                    }
                }
            }
        }

        // Métodos de consulta
        public bool IsSkillPurchased(SkillNodeData nodeData)
        {
            return isInitialized && nodeStates.ContainsKey(nodeData.name) && nodeStates[nodeData.name].isPurchased;
        }

        public bool IsSkillUnlocked(SkillNodeData nodeData)
        {
            return isInitialized && nodeStates.ContainsKey(nodeData.name) && nodeStates[nodeData.name].isUnlocked;
        }

        public int GetAvailableSkillPoints() => isInitialized ? availableSkillPoints : 0;
        public List<SkillConstellation> GetConstellations() => constellations;

        // MÉTODOS DE CONTEXT MENU - ESTOS DEBERÍAN APARECER EN EL INSPECTOR
        [ContextMenu("Add 5 Test Points")]
        public void AddTestPoints()
        {
            AddSkillPoints(5);
            Debug.Log("Added 5 test points via context menu");
        }

        [ContextMenu("Add 1 Skill Point")]
        public void AddOnePoint()
        {
            AddSkillPoints(1);
            Debug.Log("Added 1 skill point via context menu");
        }

        [ContextMenu("Debug Skill Tree State")]
        public void DebugSkillTreeState()
        {
            Debug.Log($"=== SKILL TREE DEBUG ===");
            Debug.Log($"Initialized: {isInitialized}");
            Debug.Log($"Available points: {availableSkillPoints}");
            Debug.Log($"Last cycle counted: {lastCycleCounted}");
            Debug.Log($"Current cycle (if available): {(gameTime != null ? gameTime.CurrentCycle.ToString() : "N/A")}");
            Debug.Log($"Constellations: {constellations?.Count ?? 0}");
            Debug.Log($"Node states: {nodeStates.Count}");
        }

        [ContextMenu("Reset Skill Points")]
        public void ResetSkillPoints()
        {
            availableSkillPoints = 0;
            AddSkillPoints(initialSkillPoints);
            Debug.Log("Reset skill points to initial value");
        }
    }
}