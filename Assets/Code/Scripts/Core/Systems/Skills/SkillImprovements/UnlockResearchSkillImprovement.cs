using Code.Scripts.Core.Systems.Research;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Skills.SkillImprovements
{
    [System.Serializable]
    public class UnlockResearchSkillImprovement : SkillImprovement
    {
        public string researchIdToUnlock;

        public override void ApplyImprovement()
        {
            if (string.IsNullOrEmpty(researchIdToUnlock))
            {
                return;
            }

            ResearchSystem researchSystem = ServiceLocator.GetService<ResearchSystem>();

            if (researchSystem != null)
            {
                researchSystem.UnlockResearch(researchIdToUnlock);
            }
        }
    }
}
