using System.Collections;
using System.Collections.Generic;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;

namespace Code.Scripts.UI.Menus.Trading
{
    public class TradingData
    {
        public Dictionary<ResourceType, int> ResourceData = new Dictionary<ResourceType, int>()
        {
            { ResourceType.Stone, 0 },
            { ResourceType.Metal, 0 },
            { ResourceType.Fire, 0 },
            { ResourceType.Ice, 0 },
            { ResourceType.Sand, 0 }
        };
        public List<InventoryItem> itemsToTrade = new List<InventoryItem>();
        
        public TradingData(int stone, int metal, int fire, int ice, int sand, List<InventoryItem> items)
        {
            ResourceData[ResourceType.Stone] = stone;
            ResourceData[ResourceType.Metal] = metal;
            ResourceData[ResourceType.Fire] = fire;
            ResourceData[ResourceType.Ice] = ice;
            ResourceData[ResourceType.Sand] = sand;
            itemsToTrade = items;
        }

    }
}