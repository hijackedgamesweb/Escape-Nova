namespace Code.Scripts.Core.Entity
{
    public class EntityData
    {
        public string Name;
        public string LeaderName;
        
        //Attributes
        public float AngerTolerance;
        
        public EntityData(EntitySO entitySO)
        {
            Name = entitySO.civName;
            LeaderName = entitySO.civLeaderName;
            AngerTolerance = 100f; 
        }
    }
}