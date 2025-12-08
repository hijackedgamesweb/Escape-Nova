using Code.Scripts.Core.Events;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Code.Scripts.Core.World.ConstructableEntities;
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
        
        public void AddFriendliness(float amount)
        {
            FriendlinessLevel += amount;
            if (FriendlinessLevel < 0f)
                FriendlinessLevel = 0f;
            if (FriendlinessLevel > 1)
                FriendlinessLevel = 1;
        }
        
        public void AddDependency(float amount)
        {
            DependencyLevel += amount;
            if (DependencyLevel < 0f)
                DependencyLevel = 0f;
            if (DependencyLevel > 1)
                DependencyLevel = 1;
        }
        
        public void AddInterest(float amount)
        {
            InterestLevel += amount;
            if (InterestLevel < 0f)
                InterestLevel = 0f;
            if (InterestLevel > 1)
                InterestLevel = 1;
        }
        
        public void AddTrust(float amount)
        {
            TrustLevel += amount;
            if (TrustLevel < 0f)
                TrustLevel = 0f;
            if (TrustLevel > 1)
                TrustLevel = 1;
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