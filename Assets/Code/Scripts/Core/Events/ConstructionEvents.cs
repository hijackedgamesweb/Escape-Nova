using System;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;

namespace Code.Scripts.Core.Events
{
    public static class ConstructionEvents
    {
        public static Action<ConstructibleDataSO> OnPlanetCreated;
        public static Action<SateliteDataSO> OnSateliteCreated;
        public static Action<int> OnResourceProductionChanged;
    }
}