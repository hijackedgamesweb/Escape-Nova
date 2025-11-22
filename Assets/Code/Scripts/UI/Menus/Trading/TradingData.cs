using System.Collections.Generic;
using Code.Scripts.Core.Systems.Storage;

namespace Code.Scripts.UI.Menus.Trading
{
    public class TradingData
    {
        public int stoneAmount;
        public int metalAmount;
        public int fireAmount;
        public int iceAmount;
        public int sandAmount;
        
        public List<InventoryItem> itemsToTrade = new List<InventoryItem>();
        
        public TradingData(int stone, int metal, int fire, int ice, int sand, List<InventoryItem> items)
        {
            stoneAmount = stone;
            metalAmount = metal;
            fireAmount = fire;
            iceAmount = ice;
            sandAmount = sand;
            itemsToTrade = items;
        }
    }
}