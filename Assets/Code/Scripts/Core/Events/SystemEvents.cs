using System;

namespace Code.Scripts.Core.Events
{
    public static class SystemEvents
    {
        public static event Action OnResearchUnlocked;
        public static event Action OnInventoryUnlocked;
        public static event Action OnConstellationsUnlocked;
        public static event Action OnDiplomacyUnlocked;
        
        public static event Action OnStarsPanelUnlocked;
        public static event Action OnRequestMainMenu;
        public static event Action OnGameOver; 
        
        public static bool IsResearchUnlocked { get; private set; }
        public static bool IsInventoryUnlocked { get; private set; }
        public static bool IsConstellationsUnlocked { get; private set; }
        public static bool IsDiplomacyUnlocked { get; set; }

        public static bool IsStarsPanelUnlocked { get; private set; }

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
        
        public static void UnlockConstellations()
        {
            if (IsConstellationsUnlocked) return;
            IsConstellationsUnlocked = true;
            OnConstellationsUnlocked?.Invoke();
        }
        
        public static void UnlockStarsPanel()
        {
            if (IsStarsPanelUnlocked) return;
            IsStarsPanelUnlocked = true;
            OnStarsPanelUnlocked?.Invoke();
        }
        
        public static void RequestMainMenu()
        {
            OnRequestMainMenu?.Invoke();
        }
        
        public static void TriggerGameOver()
        {
            OnGameOver?.Invoke();
        }

        public static void UnlockDiplomacyPanel()
        {
            if (IsDiplomacyUnlocked) return;
            IsDiplomacyUnlocked = true;
            OnDiplomacyUnlocked?.Invoke();
        }

        public static void UnlockAll()
        {
            UnlockResearch();
            UnlockInventory();
            UnlockConstellations();
            UnlockStarsPanel();
            UnlockDiplomacyPanel();
        }
    }
}