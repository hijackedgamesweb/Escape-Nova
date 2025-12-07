using Code.Scripts.Core.SaveLoad.Interfaces;
using Newtonsoft.Json.Linq;

namespace Code.Scripts.Core.Systems.Quests
{
    [System.Serializable]
    public abstract class QuestObjective : ISaveable
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

        public string GetSaveId()
        {
            return $"QuestObjective_{objectiveDescription.GetHashCode()}";
        }

        public JToken CaptureState()
        {
            var state = new JObject
            {
                ["description"] = objectiveDescription,
                ["isCompleted"] = isCompleted
            };
            
            return state;
        }

        public void RestoreState(JToken state)
        {
            isCompleted = state["isCompleted"].ToObject<bool>();
        }
    }
}
