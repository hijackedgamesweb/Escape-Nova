using System;
using Code.Scripts.Core.Systems.Research;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;

namespace Code.Scripts.Core.Events
{
    public static class ResearchEvents
    {
        public static Action<ResearchNode> OnResearchCompleted;
        public static Action<SateliteDataSO> OnNewSateliteResearched;
        public static Action<PlanetDataSO> OnNewPlanetResearched;

        public static void CompleteResearch(ResearchNode researchNode)
        {
            OnResearchCompleted?.Invoke(researchNode);
        }
    }
}