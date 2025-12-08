using System.Collections.Generic;
using Code.Scripts.Core.Systems.Storage;

namespace Code.Scripts.Core.Entity
{
    public class EntityItemPreferences
    {
        public float stoneWorth = 1f;
        public float metalWorth = 2f;
        public float fireWorth = 3f;
        public float iceWorth = 3f;
        public float sandWorth = 1.5f;
        
        public List<InventoryItem> tier0Items;
        public List<InventoryItem> tier2Items;
        public List<InventoryItem> tier3Items;
        public List<InventoryItem> tier4Items;
        public List<InventoryItem> tier5Items;
        
        public EntityItemPreferences(EntityItemPreferencesSO eipSO)
        {
            stoneWorth = eipSO.stoneWorth;
            metalWorth = eipSO.metalWorth;
            fireWorth = eipSO.fireWorth;
            iceWorth = eipSO.iceWorth;
            sandWorth = eipSO.sandWorth;
            
            tier0Items = eipSO.tier0Items;
            tier2Items = eipSO.tier2Items;
            tier3Items = eipSO.tier3Items;
            tier4Items = eipSO.tier4Items;
            tier5Items = eipSO.tier5Items;
        }
    }
}