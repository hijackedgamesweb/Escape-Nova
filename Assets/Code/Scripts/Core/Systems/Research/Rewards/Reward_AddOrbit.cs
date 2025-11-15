using System;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.Menus;

namespace Code.Scripts.Core.Systems.Research.Rewards
{
    [Serializable]
    public class Reward_AddOrbit : AbstractResearchReward
    {
    public override void ApplyReward()
    {
        OrbitOverview overview = ServiceLocator.GetService<OrbitOverview>();
        if (overview != null)
        {
            overview.AddNextOrbit();
        }
    }

    public override string GetDescription() => "Añadir 1 órbita";
    }
}