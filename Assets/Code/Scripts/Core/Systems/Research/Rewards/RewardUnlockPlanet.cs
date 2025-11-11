using System;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public class RewardUnlockPlanet : AbstractResearchReward
    {
        public PlanetDataSO planetData;

        public override void ApplyReward()
        {
            ResearchEvents.OnNewPlanetResearched?.Invoke(planetData);
        }
        
        public override string GetDescription() => $"Desbloquear planeta: {planetData.constructibleName}";
    }
}