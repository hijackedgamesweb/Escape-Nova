using System;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;

namespace Code.Scripts.Core.Events
{
    public static class ResearchEvents
    {
        public static Action OnResearchCompleted;
        public static Action<SateliteDataSO> OnNewSateliteResearched;
        public static Action<PlanetDataSO> OnNewPlanetResearched;
    }
}