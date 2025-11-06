using UnityEngine;

namespace Code.Scripts.Core.Systems.Quests
{
    public abstract class QuestObjective
    {
        public string ObjectiveDescription;
        public bool IsCompleted;
        
        public abstract void Initialize();
        public abstract void CheckCompletion();
        public abstract void RegisterEvents();
        public abstract void UnregisterEvents();
    }
}