using Code.Scripts.Core.Events;
using Code.Scripts.Core.Systems.Research;

namespace Code.Scripts.Core.Systems.Quests.Objectives
{
    [System.Serializable]
    public class CompleteResearchObjective : QuestObjective
    {
        public ResearchNode requiredResearch;

        public override void Initialize()
        {
            isCompleted = false;
        }

        public override void CheckCompletion()
        {
        }

        private void OnResearchCompleted(ResearchNode completedNode)
        {
            if (isCompleted || requiredResearch == null || completedNode == null)
            {
                return;
            }

            if (completedNode.researchId == requiredResearch.researchId)
            {
                isCompleted = true;
                UnregisterEvents();
            }
        }

        public override void RegisterEvents()
        {
            ResearchEvents.OnResearchCompleted += OnResearchCompleted;
        }

        public override void UnregisterEvents()
        {
            ResearchEvents.OnResearchCompleted -= OnResearchCompleted;
        }
    }
}