using System;
using System.Collections.Generic;
using Code.Scripts.Config;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Crafting;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Research
{
    public enum ResearchStatus
    {
        Locked,
        Available,
        InProgress,
        Completed
    }

    [Serializable]
    public class ResearchCost
    {
        public ResourceType resourceType;
        public int amount;
        public string itemName;
        public bool useInventoryItem;
    }

    [Serializable]
    public class ResearchPrerequisite
    {
        public string researchId;
        public string buildingId;
        public int playerLevel;
    }

    public class ResearchSystem
    {
        private Dictionary<string, ResearchNode> _researchDatabase;
        private Dictionary<string, ResearchStatus> _researchStatus;
        private Dictionary<string, ResearchData> _researchProgress;

        private StorageSystem _storageSystem;
        private IGameTime _gameTime;
        private const float STANDARD_SECONDS_PER_CYCLE = 5.0f; 

        private string _currentResearchId;
        private int _cyclesNeeded;
        private int _cyclesCompleted;

        public event Action<string> OnResearchStarted;
        public event Action<string> OnResearchCompleted;
        public event Action<string, float> OnResearchProgress;
        public event Action<string> OnResearchUnlocked;

        public ResearchSystem(List<ResearchNode> availableResearch)
        {
            ServiceLocator.RegisterService<ResearchSystem>(this);
            InitializeResearchDatabase(availableResearch);
        }

        public void InitializeDependencies()
        {
            _storageSystem = ServiceLocator.GetService<StorageSystem>();
            _gameTime = ServiceLocator.GetService<IGameTime>();
            _gameTime.OnCycleCompleted += OnCycleCompleted;
            RecalculateResearchAvailability();
        }
        
        private void OnDestroy()
        {
            if (_gameTime != null)
            {
                _gameTime.OnCycleCompleted -= OnCycleCompleted;
            }
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
                }
            }

            RecalculateResearchAvailability();
        }
        
        private void OnCycleCompleted(int currentCycle)
        {
            if (!IsAnyResearchInProgress()) return;

            _cyclesCompleted++;

            float progress = _cyclesNeeded > 0 ? Mathf.Clamp01((float)_cyclesCompleted / _cyclesNeeded) : 0f;
            _researchProgress[_currentResearchId].progress = progress;

            OnResearchProgress?.Invoke(_currentResearchId, progress);

            if (_cyclesCompleted >= _cyclesNeeded)
            {
                CompleteResearch(_currentResearchId);
            }
        }

        public bool CanStartResearch(string researchId)
        {
            if (!_researchDatabase.ContainsKey(researchId)) return false;
            if (_researchStatus[researchId] != ResearchStatus.Available) return false;
            if (IsAnyResearchInProgress()) return false;

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
            return !string.IsNullOrEmpty(_currentResearchId);
        }

        public string GetCurrentResearchId()
        {
            return _currentResearchId;
        }

        public bool StartResearch(string researchId)
        {
            if (!CanStartResearch(researchId)) return false;

            var research = _researchDatabase[researchId];
            AudioManager.Instance.PlaySFX("ResearchStarted");
            
            foreach (var cost in research.resourceCosts)
            {
                if (cost.useInventoryItem)
                    _storageSystem.ConsumeInventoryItem(cost.itemName, cost.amount);
                else
                    _storageSystem.ConsumeResource(cost.resourceType, cost.amount);
            }

            _researchStatus[researchId] = ResearchStatus.InProgress;
            _researchProgress[researchId].startTime = _gameTime.GameTime;
            _researchProgress[researchId].researchId = researchId;

            _currentResearchId = researchId;
            _cyclesNeeded = CalculateCyclesNeeded(research.researchTimeInSeconds);
            _cyclesCompleted = 0;

            OnResearchStarted?.Invoke(researchId);
            return true;
        }

        private int CalculateCyclesNeeded(float researchTimeInSeconds)
        {
            int cycles = Mathf.CeilToInt(researchTimeInSeconds / STANDARD_SECONDS_PER_CYCLE);
            return Mathf.Max(1, cycles);
        }
        
        private void CompleteResearch(string researchId)
        {
            if (!_researchDatabase.ContainsKey(researchId)) return;
            
            var researchNode = _researchDatabase[researchId];
            
            _researchStatus[researchId] = ResearchStatus.Completed;
            _researchProgress[researchId].progress = 1f;
            _researchProgress[researchId].completionTime = _gameTime.GameTime;

            ApplyResearchRewards(researchId);
            UnlockNewResearch(researchId);
            
            AudioManager.Instance.PlaySFX("ResearchFinished");

            _currentResearchId = null;
            _cyclesNeeded = 0;
            _cyclesCompleted = 0;

            OnResearchCompleted?.Invoke(researchId);
            Code.Scripts.Core.Events.ResearchEvents.CompleteResearch(researchNode);
            NotificationManager.Instance.CreateNotification($"Research: {researchNode.name} completed", NotificationType.Info);
        }

        public bool CancelCurrentResearch()
        {
            if (!IsAnyResearchInProgress()) return false;

            string researchToCancelId = _currentResearchId;
            _researchStatus[researchToCancelId] = ResearchStatus.Available;

            if (_researchProgress.TryGetValue(researchToCancelId, out ResearchData data))
            {
                data.progress = 0f;
                data.startTime = 0f;
            }

            _currentResearchId = null;
            _cyclesNeeded = 0;
            _cyclesCompleted = 0;

            return true;
        }

        private void ApplyResearchRewards(string researchId)
        {
            var research = _researchDatabase[researchId];
            foreach (var reward in research.rewards)
            {
                reward.ApplyReward();
            }
        }

        private void UnlockNewResearch(string completedResearchId)
        {
            var research = _researchDatabase[completedResearchId];
            foreach (var unlockedId in research.unlocksResearchIds)
            {
                UnlockResearch(unlockedId);
            }
        }

        public void UnlockResearch(string researchId)
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
            }
            return true;
        }

        public ResearchStatus GetResearchStatus(string researchId) => _researchStatus.GetValueOrDefault(researchId, ResearchStatus.Locked);
        public float GetResearchProgress(string researchId) => _researchProgress.GetValueOrDefault(researchId, new ResearchData()).progress;
        public bool IsResearchCompleted(string researchId) => GetResearchStatus(researchId) == ResearchStatus.Completed;
        public bool IsResearchAvailable(string researchId) => GetResearchStatus(researchId) == ResearchStatus.Available;
        public ResearchNode GetResearch(string researchId) => _researchDatabase.GetValueOrDefault(researchId);
        public Dictionary<string, ResearchStatus> GetAllResearchStatus() => new Dictionary<string, ResearchStatus>(_researchStatus);
    }

    [System.Serializable]
    public class ResearchData
    {
        public string researchId;
        public float startTime;
        public float completionTime;
        public float progress;
    }
}