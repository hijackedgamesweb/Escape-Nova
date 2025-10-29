namespace Code.Scripts.Core.Entity
{
    public class EntityState
    {
        public float HungerLevel { get; set; }
        public float AngerLevel { get; set; }
        public float MilitaryPowerLevel { get; set; }
        
        public EntityState(EntitySO entitySO)
        {
            HungerLevel = entitySO.baseHunger;
            AngerLevel = entitySO.baseAnger;
            MilitaryPowerLevel = entitySO.baseMilitaryPower;
        }
        
    }
}