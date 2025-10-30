// ResearchSystem.cs
using System;
using System.Collections.Generic;
using System.Collections;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
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
        ModifyStat           // Modificar estadísticas del jugador
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
        private Coroutine _currentResearchCoroutine;
        private string _currentResearchId;
        
        // Eventos
        public event Action<string> OnResearchStarted;          // researchId
        public event Action<string> OnResearchCompleted;        // researchId
        public event Action<string, float> OnResearchProgress;  // researchId, progress (0-1)
        public event Action<string> OnResearchUnlocked;         // researchId
        
        public ResearchSystem(List<ResearchNode> availableResearch)
        {
            ServiceLocator.RegisterService<ResearchSystem>(this);
            _storageSystem = ServiceLocator.GetService<StorageSystem>();
            InitializeResearchDatabase(availableResearch);
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
        
            // AÑADIDO: Verificar que no hay otra investigación en curso
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
            return _currentResearchCoroutine != null && !string.IsNullOrEmpty(_currentResearchId);
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
            _researchProgress[researchId].startTime = DateTime.Now;
            _researchProgress[researchId].researchId = researchId;
        
            // AÑADIDO: Asignar como investigación actual
            _currentResearchId = researchId;
        
            // Iniciar corrutina para el tiempo de investigación
            if (_currentResearchCoroutine != null)
            {
                StopCurrentResearch();
            }
        
            _currentResearchCoroutine = ResearchCoroutineRunner.StartCoroutine(ResearchCoroutine(researchId));
        
            OnResearchStarted?.Invoke(researchId);
            Debug.Log($"Research started: {researchId}");
        
            return true;
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
            _researchProgress[researchId].completionTime = DateTime.Now;
        
            // AÑADIDO: Limpiar investigación actual
            _currentResearchId = null;
        
            // Aplicar recompensas
            ApplyResearchRewards(researchId);
        
            // Desbloquear nuevas investigaciones
            UnlockNewResearch(researchId);
        
            _currentResearchCoroutine = null;
        
            OnResearchCompleted?.Invoke(researchId);
            Debug.Log($"Research completed: {researchId}");
        }
        
        public bool CancelCurrentResearch()
        {
            if (!IsAnyResearchInProgress()) return false;
        
            ResearchCoroutineRunner.StopCoroutine(_currentResearchCoroutine);
            _currentResearchId = null;
            _currentResearchCoroutine = null;
        
            // Revertir estado a Available
            if (!string.IsNullOrEmpty(_currentResearchId))
            {
                _researchStatus[_currentResearchId] = ResearchStatus.Available;
            }
        
            Debug.Log("Investigación cancelada");
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
                }
            }
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
        
        // Métodos de consulta
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
        
        private void StopCurrentResearch()
        {
            if (_currentResearchCoroutine != null)
            {
                ResearchCoroutineRunner.StopCoroutine(_currentResearchCoroutine);
                _currentResearchCoroutine = null;
            }
        }
    }
    
    // Clase auxiliar para datos de progreso
    [System.Serializable]
    public class ResearchData
    {
        public string researchId;
        public DateTime startTime;
        public DateTime completionTime;
        public float progress;
    }
}

public static class ResearchCoroutineRunner
{
    private static ResearchCoroutineMonoBehaviour _runner;
    
    static ResearchCoroutineRunner()
    {
        var gameObject = new GameObject("ResearchCoroutineRunner");
        _runner = gameObject.AddComponent<ResearchCoroutineMonoBehaviour>();
        UnityEngine.Object.DontDestroyOnLoad(gameObject);
    }
    
    public static Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return _runner.StartCoroutine(coroutine);
    }
    
    public static void StopCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            _runner.StopCoroutine(coroutine);
        }
    }
    
    private class ResearchCoroutineMonoBehaviour : MonoBehaviour { }
}