using Code.Scripts.Core.Events;

namespace Code.Scripts.Core.Systems.Quests.Objectives
{
    public class CraftItem : QuestObjective
    {
        public ItemData requiredItem;
        public override void Initialize()
        {
            isCompleted = false;
        }

        public override void CheckCompletion()
        {
        }


        private void OnItemCrafted(ItemData obj)
        {
            if(obj.displayName == requiredItem.displayName)
            {
                isCompleted = true;
                UnregisterEvents();
            }
        }
        
        public override void RegisterEvents()
        {
            CraftingEvents.OnItemCrafted += OnItemCrafted;
        }

        public override void UnregisterEvents()
        {
            CraftingEvents.OnItemCrafted -= OnItemCrafted;
        }
    }
}