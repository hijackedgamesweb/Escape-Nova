using System;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public abstract class AbstractResearchReward
    {
        public abstract void ApplyReward();
        public virtual string GetDescription() => GetType().Name;
    }
}