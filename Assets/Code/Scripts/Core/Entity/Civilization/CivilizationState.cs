using Code.Scripts.Core.Events;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Newtonsoft.Json.Linq;

namespace Code.Scripts.Core.Entity.Civilization
{
    public class CivilizationState : EntityState, ISaveable
    {
        public CivilizationState(CivilizationSO entitySO) : base(entitySO)
        {
            
        }
        
        public CivilizationState() : base()
        {
            
        }

        public void SetCurrentMood(EntityMood mood)
        {
            CurrentMood = mood;
            UIEvents.OnUpdateCivilizationUI?.Invoke();
        }

        public string GetSaveId()
        {
            return $"CivilizationState_{entityName}";
        }

        public JToken CaptureState()
        {
            JObject obj = new JObject
            {
                ["FriendlinessLevel"] = FriendlinessLevel,
                ["DependencyLevel"] = DependencyLevel,
                ["InterestLevel"] = InterestLevel,
                ["TrustLevel"] = TrustLevel,
                ["CurrentMood"] = (int)CurrentMood
            };
            return obj;
        }

        public void RestoreState(JToken state)
        {
            JObject obj = state as JObject;
            FriendlinessLevel = obj["FriendlinessLevel"].ToObject<float>();
            DependencyLevel = obj["DependencyLevel"].ToObject<float>();
            InterestLevel = obj["InterestLevel"].ToObject<float>();
            TrustLevel = obj["TrustLevel"].ToObject<float>();
            CurrentMood = (EntityMood)obj["CurrentMood"].ToObject<int>();
        }
    }
}