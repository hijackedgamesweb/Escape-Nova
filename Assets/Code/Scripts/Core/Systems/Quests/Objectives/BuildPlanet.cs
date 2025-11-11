using Code.Scripts.Core.Events;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests.Objectives
{
    [System.Serializable]
    public class BuildConstructible : QuestObjective
    {
        public ConstructibleDataSO requiredConstructible;
        public override void Initialize()
        {
            isCompleted = false;
        }

        public override void CheckCompletion()
        {
        }

        private void OnConstructibleCreated(ConstructibleDataSO obj)
        {
            if(obj.constructibleName == requiredConstructible.constructibleName)
            {
                isCompleted = true;
                UnregisterEvents();
            }
        }

        public override void RegisterEvents()
        {
            ConstructionEvents.OnConstructibleCreated += OnConstructibleCreated;
        }

        public override void UnregisterEvents()
        {
            ConstructionEvents.OnConstructibleCreated -= OnConstructibleCreated;
        }
    }
}