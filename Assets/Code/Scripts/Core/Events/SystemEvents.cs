using System;
using Code.Scripts.Core.Entity.Civilization;

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
        
        public static event Action<Civilization> OnWarDeclaredToPlayer;
        public static event Action<Civilization> OnPeaceSigned;
        public static event Action<int, int> OnWarHealthUpdated;

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
        
        public static void LockAll()
        {
            IsResearchUnlocked = false;
            IsInventoryUnlocked = false;
            IsConstellationsUnlocked = false;
            IsStarsPanelUnlocked = false;
            IsDiplomacyUnlocked = false;
        }
        
        public static void RequestMainMenu() => OnRequestMainMenu?.Invoke();
        public static void TriggerGameOver() => OnGameOver?.Invoke();

        public static void TriggerWarDeclared(Civilization aggressor) 
        {
            OnWarDeclaredToPlayer?.Invoke(aggressor);
        }

        public static void TriggerPeaceSigned(Civilization civ) 
        {
            OnPeaceSigned?.Invoke(civ);
        }

        public static void TriggerWarHealthUpdated(int enemyHealth, int playerHealth) 
        {
            OnWarHealthUpdated?.Invoke(enemyHealth, playerHealth);
        }
    }
}