using Code.Scripts.Core.Events;

namespace Code.Scripts.Core.Entity
{
    public class EntityState
    {
        public float FriendlinessLevel { get; set; }
        public float DependencyLevel { get; set; }
        public float InterestLevel { get; set; }
        public float TrustLevel { get; set; }
        
        public EntityState(EntitySO entitySO)
        {
            FriendlinessLevel = entitySO.baseFriendship;
            DependencyLevel = entitySO.baseDependency;
            InterestLevel = entitySO.baseInterest;
            TrustLevel = entitySO.baseTrust;
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