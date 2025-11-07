namespace Code.Scripts.Core.Systems.Quests
{
    [System.Serializable]
    public abstract class QuestObjective
    {
        public string objectiveDescription;
        public bool isCompleted;
        
        public abstract void Initialize();
        public abstract void CheckCompletion();
        public abstract void RegisterEvents();
        public abstract void UnregisterEvents();

        public QuestObjective Clone()
        {
            return (QuestObjective)this.MemberwiseClone();
        }
    }
}
