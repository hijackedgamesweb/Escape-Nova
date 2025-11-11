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
        
    }
}