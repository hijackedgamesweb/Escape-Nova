using System;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Systems.Resources;

namespace Code.Scripts.Core.Systems.Quests.Objectives
{
    [Serializable]
    public class ProduceResource : QuestObjective
    {
        public ResourceType resourceType;
        public int requiredAmount;
        private int _currentAmount;
        public override void Initialize()
        {
            _currentAmount = 0;
        }

        public override void CheckCompletion()
        {
            if (_currentAmount >= requiredAmount)
            {
                isCompleted = true;
                UnregisterEvents();
            }
        }

        public override void RegisterEvents()
        {
            ConstructionEvents.OnResourceProductionAdded += OnResourceProductionChanged;
        }

        private void OnResourceProductionChanged(int currentProduction, ResourceType type)
        {
            if (type != resourceType) return;
            _currentAmount += currentProduction;
            CheckCompletion();
        }

        public override void UnregisterEvents()
        {
            ConstructionEvents.OnResourceProductionAdded -= OnResourceProductionChanged;
        }
    }
}