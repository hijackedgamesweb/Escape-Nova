using System;
using Code.Scripts.Core.Events;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public class UnlockConstellationsReward : AbstractResearchReward
    {
        public override void ApplyReward()
        {
            SystemEvents.UnlockConstellations();
        }

        public override string GetDescription()
        {
            return "Desbloquea el Árbol de Constelaciones";
        }
    }
}