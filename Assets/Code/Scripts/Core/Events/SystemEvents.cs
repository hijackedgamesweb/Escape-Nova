using System;

namespace Code.Scripts.Core.Events
{
    public class SystemEvents
    {
        public static Action OnResearchUnlocked;
        public static Action OnInventoryUnlocked;
        
        public static bool IsResearchUnlocked { get; private set; }
        public static bool IsInventoryUnlocked { get; private set; }

        public static void UnlockResearch()
        {
            if (IsResearchUnlocked) return;
            IsResearchUnlocked = true;
            OnResearchUnlocked?.Invoke();
        }

        public static void UnlockInventory()
        {
            if (IsInventoryUnlocked) return;
            IsInventoryUnlocked = true;
            OnInventoryUnlocked?.Invoke();
        }
    }
}