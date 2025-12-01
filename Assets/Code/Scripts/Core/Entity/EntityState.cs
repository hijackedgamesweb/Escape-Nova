using Code.Scripts.Core.Events;

namespace Code.Scripts.Core.Entity
{
    public enum EntityMood
    {
        Disgusted,
        Progressive,
        Needed,
        Ally,
        Commerce,
        Love,
        Negotiation,
        Belligerent,
        Peaceful,
        Generous,
        Offended
    }
    public class EntityState
    {
        public float FriendlinessLevel { get; set; }
        public float DependencyLevel { get; set; }
        public float InterestLevel { get; set; }
        public float TrustLevel { get; set; }
        
        public EntityMood CurrentMood { get; set; }
        
        public EntityState(EntitySO entitySO)
        {
            FriendlinessLevel = entitySO.baseFriendship;
            DependencyLevel = entitySO.baseDependency;
            InterestLevel = entitySO.baseInterest;
            TrustLevel = entitySO.baseTrust;
        }
        
        public string GetMoodDescription()
        {
            switch (CurrentMood)
            {
                case EntityMood.Disgusted:
                    return "Disgusted";
                case EntityMood.Progressive:
                    return "Optimistic";
                case EntityMood.Needed:
                    return "Required";
                case EntityMood.Ally:
                    return "Ally";
                case EntityMood.Commerce:
                    return "Commercial";
                case EntityMood.Love:
                    return "Loving";
                case EntityMood.Negotiation:
                    return "Negotiating";
                case EntityMood.Belligerent:
                    return "Aggressive";
                case EntityMood.Peaceful:
                    return "Peaceful";
                case EntityMood.Generous:
                    return "Generous";
                case EntityMood.Offended:
                    return "Offended";
                default:
                    return "Neutral";
                
            }
        }

        public void ReceiveBlame()
        {
            FriendlinessLevel -= 0.1f;
            TrustLevel -= 0.05f;
            if (FriendlinessLevel < 0f) FriendlinessLevel = 0f;
            if (TrustLevel < 0f) TrustLevel = 0f;
            
            UIEvents.OnUpdateCivilizationUI?.Invoke();
        }
        
        
        public void ReceiveBond()
        {
            FriendlinessLevel = 1.0f;
            TrustLevel = 1.0f;
            DependencyLevel = 1.0f;
            InterestLevel = 1.0f;
            UIEvents.OnUpdateCivilizationUI?.Invoke();
        }
    }
}