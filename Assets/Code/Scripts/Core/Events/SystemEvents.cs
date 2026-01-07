using System;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.World.ConstructableEntities;

namespace Code.Scripts.Core.Events
{
    public enum WarResult
    {
        Victory, // Ganamos nosotros
        Defeat   // Gana la IA (nos conquista)
    }
    public static class SystemEvents
    {
        public static event Action OnResearchUnlocked;
        public static Action<Civilization> OnWarDeclaredToPlayer;
        public static event Action OnInventoryUnlocked;
        public static event Action OnConstellationsUnlocked;
        public static event Action OnDiplomacyUnlocked;
        public static event Action OnStarsPanelUnlocked;
        public static event Action OnRequestMainMenu;
        public static event Action OnGameOver; 
        
        public static event Action<Civilization, WarResult> OnPeaceSigned;
        public static event Action<int, int> OnWarHealthUpdated;
        public static event Action<Planet> OnWarStarted;
        public static event Action<Planet> OnWarWon;
        public static event Action<Planet> OnWarLost;

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

        public static void TriggerPeaceSigned(Civilization civ, WarResult result) 
        {
            OnPeaceSigned?.Invoke(civ, result); 
        }

        public static void TriggerWarHealthUpdated(int enemyHealth, int playerHealth) 
        {
            OnWarHealthUpdated?.Invoke(enemyHealth, playerHealth);
        }
        public static void TriggerWarStarted(Planet planet)
        {
            OnWarStarted?.Invoke(planet);
        }

        public static void TriggerWarWon(Planet planet)
        {
            OnWarWon?.Invoke(planet);
        }

        public static void TriggerWarLost(Planet planet)
        {
            OnWarLost?.Invoke(planet);
        }
    }
}