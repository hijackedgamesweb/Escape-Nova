using System.Collections.Generic;

namespace Code.Scripts.Core.Systems.Skills
{
    public static class SkillImprovementTracker
    {
        private static HashSet<string> _purchasedSkillIds = new HashSet<string>();
        private static HashSet<string> _appliedGlobalBuffs = new HashSet<string>();

        public static void MarkSkillAsPurchased(string skillId)
        {
            _purchasedSkillIds.Add(skillId);
        }

        public static bool IsSkillPurchased(string skillId)
        {
            return _purchasedSkillIds.Contains(skillId);
        }

        public static bool TryApplyGlobalBuff(string buffId)
        {
            return _appliedGlobalBuffs.Add(buffId);
        }
    }
}