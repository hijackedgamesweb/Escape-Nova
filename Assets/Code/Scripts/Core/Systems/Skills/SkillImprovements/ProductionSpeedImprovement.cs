using Code.Scripts.Core.World.ConstructableEntities;

namespace Code.Scripts.Core.Systems.Skills.SkillImprovements
{
    [System.Serializable]
    public class ProductionSpeedImprovement : SkillImprovement
    {
        public float productionBonusPercent;

        public override void ApplyImprovement()
        {
            // Usar el método estático de Planet para aplicar mejora global
            Planet.AddGlobalImprovement("GeneralProduction", productionBonusPercent);
        }
    }
}