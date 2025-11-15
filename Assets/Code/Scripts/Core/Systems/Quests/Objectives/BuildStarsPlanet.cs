using Code.Scripts.Core.Events;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;

namespace Code.Scripts.Core.Systems.Quests.Objectives
{
    [System.Serializable]
    public class BuildStarsPlanet : QuestObjective
    {
        public ConstructibleDataSO starsPlanetData;

        public override void Initialize()
        {
            isCompleted = false;
        }

        public override void CheckCompletion()
        {
        }

        private void OnConstructibleCreated(ConstructibleDataSO obj)
        {
            if (isCompleted) return;

            if (obj.constructibleName == starsPlanetData.constructibleName)
            {
                isCompleted = true;
                
                SystemEvents.UnlockResearch();
                SystemEvents.UnlockInventory();
                
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