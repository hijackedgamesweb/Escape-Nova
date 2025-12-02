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
            { ResourceType.Batee, 0 },
            { ResourceType.Paladium, 0 },
            { ResourceType.Magmavite, 0 },
            { ResourceType.Frostice, 0 },
            { ResourceType.Sandit, 0 }
        };
        public List<InventoryItem> itemsToTrade = new List<InventoryItem>();
        
        public TradingData(int batee, int paladium, int magmavite, int frostice, int sandit, List<InventoryItem> items)
        {
            ResourceData[ResourceType.Batee] = batee;
            ResourceData[ResourceType.Paladium] = paladium;
            ResourceData[ResourceType.Magmavite] = magmavite;
            ResourceData[ResourceType.Frostice] = frostice;
            ResourceData[ResourceType.Sandit] = sandit;
            itemsToTrade = items;
        }

    }
}