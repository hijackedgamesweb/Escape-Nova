using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using Code.Scripts.Patterns.ServiceLocator;

namespace Code.Scripts.Core.Systems.Skills.SkillImprovements
{
    [System.Serializable]
    public class StorageCapacityImprovement : SkillImprovement
    {
        public int additionalCapacity;
        public ResourceType resourceType;

        public override void ApplyImprovement()
        {
            ServiceLocator.GetService<StorageSystem>().AddMaxCapacity(resourceType, additionalCapacity);
        }
    }

}