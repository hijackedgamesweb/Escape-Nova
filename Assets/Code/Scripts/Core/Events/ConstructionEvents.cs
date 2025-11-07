using System;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;

namespace Code.Scripts.Core.Events
{
    public static class ConstructionEvents
    {
        public static Action<ConstructibleDataSO> OnConstructibleCreated;
        public static Action<int, ResourceType> OnResourceProductionAdded;
    }
}