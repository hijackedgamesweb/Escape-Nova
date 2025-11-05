// ResearchSystem.cs
using System;
using System.Collections.Generic;
using System.Collections;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.Systems.Time;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Research
{
    public enum ResearchRewardType
    {
        IncreaseMaxStack,    // Aumentar capacidad máxima de un recurso
        UnlockBuilding,      // Desbloquear nueva construcción
        UnlockSpecies,       // Desbloquear nueva especie
        UnlockMission,       // Desbloquear nueva misión
        UnlockTechnology,    // Desbloquear otra tecnología
        ModifyStat,           // Modificar estadísticas del jugador
        UnlockCraftingRecipe,
        AddItemToInventory
        
    }

    // Estados de una investigación
    public enum ResearchStatus
    {
        Locked,     // No disponible (requisitos no cumplidos)
        Available,  // Disponible para investigar
        InProgress, // En investigación
        Completed   // Completada
    }

    [System.Serializable]
    public class ResearchReward
    {
        public ResearchRewardType rewardType;
        public string targetId;      // ID del recurso, construcción, especie, etc.
        public int value;           // Valor del bonus (ej: +900 de maxStack)
        public string displayName;  // Nombre para UI
    }

    [System.Serializable]
    public class ResearchCost
    {
        public ResourceType resourceType;
        public int amount;
        public string itemName;     // Para items de inventario
        public bool useInventoryItem; // Si usa recurso o item de inventario
    }

    [System.Serializable]
    public class ResearchPrerequisite
    {
        public string researchId;   // ID de investigación requerida
        public string buildingId;   // ID de construcción requerida
        public int playerLevel;     // Nivel de jugador requerido
    }
    public class ResearchSystem
    {
        private Dictionary<string, ResearchNode> _researchDatabase;
        private Dictionary<string, ResearchStatus> _researchStatus;
        private Dictionary<string, ResearchData> _researchProgress;
        
        private StorageSystem _storageSystem;
        private TimeScheduler _timeScheduler;
        private ITimerHandle _currentResearchTimer;
        private IGameTime _gameTime;
        private string _currentResearchId;
        
        // Eventos
        public event Action<string> OnResearchStarted;          // researchId
        public event Action<string> OnResearchCompleted;        // researchId
        public event Action<string, float> OnResearchProgress;  // researchId, progreso (0-1)
        public event Action<string> OnResearchUnlocked;         // researchId
        
        public ResearchSystem(List<ResearchNode> availableResearch)
        {
            ServiceLocator.RegisterService<ResearchSystem>(this);
            InitializeResearchDatabase(availableResearch);
        }
        
        public void InitializeDependencies()
        {
            _storageSystem = ServiceLocator.GetService<StorageSystem>();
            _timeScheduler = ServiceLocator.GetService<TimeScheduler>();
            _gameTime = ServiceLocator.GetService<IGameTime>();
        
            RecalculateResearchAvailability(); 
        }
        
        private void InitializeResearchDatabase(List<ResearchNode> availableResearch)
        {
            _researchDatabase = new Dictionary<string, ResearchNode>();
            _researchStatus = new Dictionary<string, ResearchStatus>();
            _researchProgress = new Dictionary<string, ResearchData>();
            
            foreach (var research in availableResearch)
            {
                if (research != null && !string.IsNullOrEmpty(research.researchId))
                {
                    _researchDatabase[research.researchId] = research;
                    _researchStatus[research.researchId] = ResearchStatus.Locked;
                    _researchProgress[research.researchId] = new ResearchData();
                    
                    Debug.Log($"Research registered: {research.researchId}");
                }
            }
            
            // Recalcular disponibilidad después de inicializar todo
            RecalculateResearchAvailability();
        }
        
        public bool CanStartResearch(string researchId)
        {
            if (!_researchDatabase.ContainsKey(researchId)) return false;
            if (_researchStatus[researchId] != ResearchStatus.Available) return false;
        
            if (IsAnyResearchInProgress())
            {
                Debug.Log($"Ya hay una investigación en curso: {_currentResearchId}");
                return false;
            }
        
            var research = _researchDatabase[researchId];
        
            foreach (var cost in research.resourceCosts)
            {
                if (cost.useInventoryItem)
                {
                    if (!_storageSystem.HasInventoryItem(cost.itemName, cost.amount))
                        return false;
                }
                else
                {
                    if (!_storageSystem.HasResource(cost.resourceType, cost.amount))
                        return false;
                }
            }
        
            return true;
        }
        
        public bool IsAnyResearchInProgress()
        {
            //si el timer es nulo, significa que no hay investigacion en curso.
            //Aunque el timer podria estar creado y cancelado, el StartResearch 
            //y CancelCurrentResearch se encargaran de mantenerlo limpio
            return _currentResearchTimer != null && !string.IsNullOrEmpty(_currentResearchId);
        }
        
        public string GetCurrentResearchId()
        {
            return _currentResearchId;
        }
        
        public bool StartResearch(string researchId)
        {
            if (!CanStartResearch(researchId)) return false;
        
            var research = _researchDatabase[researchId];
        
            // Consumir recursos
            foreach (var cost in research.resourceCosts)
            {
                if (cost.useInventoryItem)
                {
                    _storageSystem.ConsumeInventoryItem(cost.itemName, cost.amount);
                }
                else
                {
                    _storageSystem.ConsumeResource(cost.resourceType, cost.amount);
                }
            }
        
            // Iniciar investigación
            _researchStatus[researchId] = ResearchStatus.InProgress;
            _researchProgress[researchId].startTime = _gameTime.GameTime;
            _researchProgress[researchId].researchId = researchId;
        
            // AÑADIDO: Asignar como investigación actual
            _researchStatus[researchId] = ResearchStatus.InProgress;
            _currentResearchId = researchId;
        
            // Iniciar corrutina para el tiempo de investigación
            if (_currentResearchTimer != null)
            {
                _currentResearchTimer.Cancel(); 
            }
        
            const float UPDATE_INTERVAL = 1f;
            
            Action updateResearchAction = () => UpdateResearchProgress(researchId, UPDATE_INTERVAL);
            _currentResearchTimer = _timeScheduler.ScheduleRepeating(UPDATE_INTERVAL, updateResearchAction);
        
            OnResearchStarted?.Invoke(researchId);
            Debug.Log($"Research started: {researchId}");
        
            return true;
        }
        private void UpdateResearchProgress(string researchId, float deltaTime)
        {
            if (!_researchDatabase.ContainsKey(researchId) || _researchStatus[researchId] != ResearchStatus.InProgress)
            {
                CancelCurrentResearch();
                return;
            }
    
            var research = _researchDatabase[researchId];
            var data = _researchProgress[researchId];
            float researchTime = research.researchTimeInSeconds;
    
            float timeElapsedSinceStart = _gameTime.GameTime - data.startTime;

            if (timeElapsedSinceStart >= researchTime)
            {
                float progress = 1f;
                data.progress = progress;
                OnResearchProgress?.Invoke(researchId, progress);
        
                CompleteResearch(researchId);
            }
            else
            {
                float progress = Mathf.Clamp01(timeElapsedSinceStart / researchTime);
                data.progress = progress;
                OnResearchProgress?.Invoke(researchId, progress);
            }
        }
        
        private IEnumerator ResearchCoroutine(string researchId)
        {
            var research = _researchDatabase[researchId];
            float researchTime = research.researchTimeInSeconds;
            float elapsedTime = 0f;

            while (elapsedTime < researchTime)
            {
                // Esperar exactamente 1 segundo y actualizar
                yield return new WaitForSeconds(1f);
                elapsedTime += 1f;

                float progress = Mathf.Clamp01(elapsedTime / researchTime);

                _researchProgress[researchId].progress = progress;
                OnResearchProgress?.Invoke(researchId, progress);
            }

            CompleteResearch(researchId);
        }
        
        private void CompleteResearch(string researchId)
        {
            _researchStatus[researchId] = ResearchStatus.Completed;
            _researchProgress[researchId].progress = 1f;
            _researchProgress[researchId].completionTime = _gameTime.GameTime;
        
            _currentResearchId = null;
        
            ApplyResearchRewards(researchId);
        
            UnlockNewResearch(researchId);
        
            OnResearchCompleted?.Invoke(researchId);
            Debug.Log($"Research completed: {researchId}");
        }
        

        public bool CancelCurrentResearch()
        {
            if (!IsAnyResearchInProgress()) return false;

            string researchToCancelId = _currentResearchId; 
    
            if (_currentResearchTimer != null)
            {
                _currentResearchTimer.Cancel();
            }

            _currentResearchTimer = null;
            _currentResearchId = null; 

            if (!string.IsNullOrEmpty(researchToCancelId))
            {
                _researchStatus[researchToCancelId] = ResearchStatus.Available;
        
                if (_researchProgress.TryGetValue(researchToCancelId, out ResearchData data))
                {
                    data.progress = 0f;
                    data.startTime = 0f;
                }
                //si por cualquier motivo queremos devolver los recursos, eso lo ponemos aqi
            }

            Debug.Log($"Investigación cancelada: {researchToCancelId}");
            return true;
        }
        
        private void ApplyResearchRewards(string researchId)
        {
            var research = _researchDatabase[researchId];
            
            foreach (var reward in research.rewards)
            {
                switch (reward.rewardType)
                {
                    case ResearchRewardType.IncreaseMaxStack:
                        ApplyMaxStackIncrease(reward);
                        break;
                    case ResearchRewardType.UnlockBuilding:
                        ApplyBuildingUnlock(reward);
                        break;
                    case ResearchRewardType.UnlockSpecies:
                        ApplySpeciesUnlock(reward);
                        break;
                    case ResearchRewardType.UnlockMission:
                        ApplyMissionUnlock(reward);
                        break;
                    case ResearchRewardType.UnlockTechnology:
                        ApplyTechnologyUnlock(reward);
                        break;
                    case ResearchRewardType.ModifyStat:
                        ApplyStatModification(reward);
                        break;
                    case ResearchRewardType.UnlockCraftingRecipe:
                        ApplyCraftingUnlock(reward);
                        break;
                    case ResearchRewardType.AddItemToInventory:
                        ApplyAddItemToInventory(reward);
                        break;
                }
            }
        }
        
        private void ApplyAddItemToInventory(ResearchReward reward)
        {
            StorageSystem storage = ServiceLocator.GetService<StorageSystem>();
            if (storage == null)
            {
                Debug.LogError("StorageSystem no encontrado al intentar dar recompensa de item.");
                return;
            }
            string itemName = reward.targetId;
            int amountToAdd = reward.value;

            if (amountToAdd <= 0) amountToAdd = 1;

            if (storage.AddInventoryItem(itemName, amountToAdd))
            {
                Debug.Log($"Recompensa: Añadido {amountToAdd}x {itemName} al inventario.");
            }
            else
            {
                Debug.LogWarning($"Recompensa: Se intentó añadir {itemName} pero el item no existe en el StorageSystem.");
            }
        }
        
        private void ApplyCraftingUnlock(ResearchReward reward)
        {
            Debug.Log($"Recompensa: Desbloqueando receta de crafteo - ID: {reward.targetId}");
            
            //ALGO asi
            /*
            CraftingSystem craftingSystem = ServiceLocator.GetService<CraftingSystem>();
            if (craftingSystem != null)
            {
                craftingSystem.UnlockRecipe(reward.targetId);
            }
            else
            {
                Debug.LogError($"No se pudo encontrar el CraftingSystem para desbloquear la receta: {reward.targetId}");
            }
            */
        }
        
        private void ApplyMaxStackIncrease(ResearchReward reward)
        {
            Debug.Log($"Max stack increased for {reward.targetId} by {reward.value}");
            // _storageSystem.IncreaseMaxStack(reward.targetId, reward.value);
        }
        
        private void ApplyBuildingUnlock(ResearchReward reward)
        {
            // Conectar con sistema de construcción
            Debug.Log($"Building unlocked: {reward.targetId}");
        }
        
        private void ApplySpeciesUnlock(ResearchReward reward)
        {
            // Conectar con sistema de especies
            Debug.Log($"Species unlocked: {reward.targetId}");
        }
        
        private void ApplyMissionUnlock(ResearchReward reward)
        {
            // Conectar con sistema de misiones
            Debug.Log($"Mission unlocked: {reward.targetId}");
        }
        
        private void ApplyTechnologyUnlock(ResearchReward reward)
        {
            // Desbloquear otra tecnología
            UnlockResearch(reward.targetId);
        }
        
        private void ApplyStatModification(ResearchReward reward)
        {
            // Modificar estadísticas del jugador
            Debug.Log($"Stat modified: {reward.targetId} by {reward.value}");
        }
        
        private void UnlockNewResearch(string completedResearchId)
        {
            var research = _researchDatabase[completedResearchId];
            
            foreach (var unlockedId in research.unlocksResearchIds)
            {
                UnlockResearch(unlockedId);
            }
        }
        
        private void UnlockResearch(string researchId)
        {
            if (_researchDatabase.ContainsKey(researchId) && 
                _researchStatus[researchId] == ResearchStatus.Locked)
            {
                _researchStatus[researchId] = ResearchStatus.Available;
                OnResearchUnlocked?.Invoke(researchId);
            }
        }
        
        public void RecalculateResearchAvailability()
        {
            foreach (var research in _researchDatabase.Values)
            {
                if (_researchStatus[research.researchId] == ResearchStatus.Locked)
                {
                    if (CheckPrerequisites(research))
                    {
                        _researchStatus[research.researchId] = ResearchStatus.Available;
                        OnResearchUnlocked?.Invoke(research.researchId);
                    }
                }
            }
        }
        
        private bool CheckPrerequisites(ResearchNode research)
        {
            foreach (var prereq in research.prerequisites)
            {
                if (!string.IsNullOrEmpty(prereq.researchId))
                {
                    if (!IsResearchCompleted(prereq.researchId))
                        return false;
                }
                
                if (!string.IsNullOrEmpty(prereq.buildingId))
                {
                    // Verificar con sistema de construcción
                    // if (!_buildingSystem.HasBuilding(prereq.buildingId)) return false;
                }
                
                if (prereq.playerLevel > 0)
                {
                    // Verificar con sistema de progresión
                    // if (_progressionSystem.GetPlayerLevel() < prereq.playerLevel) return false;
                }
            }
            return true;
        }
        
        public ResearchStatus GetResearchStatus(string researchId)
        {
            return _researchStatus.GetValueOrDefault(researchId, ResearchStatus.Locked);
        }
        
        public float GetResearchProgress(string researchId)
        {
            return _researchProgress.GetValueOrDefault(researchId, new ResearchData()).progress;
        }
        
        public bool IsResearchCompleted(string researchId)
        {
            return GetResearchStatus(researchId) == ResearchStatus.Completed;
        }
        
        public bool IsResearchAvailable(string researchId)
        {
            return GetResearchStatus(researchId) == ResearchStatus.Available;
        }
        
        public ResearchNode GetResearch(string researchId)
        {
            return _researchDatabase.GetValueOrDefault(researchId);
        }
        
        public Dictionary<string, ResearchStatus> GetAllResearchStatus()
        {
            return new Dictionary<string, ResearchStatus>(_researchStatus);
        }
    }
    
    // Clase auxiliar para datos de progreso
    [System.Serializable]
    public class ResearchData
    {
        public string researchId;
        public float startTime;
        public float completionTime;
        public float progress;
    }
}
