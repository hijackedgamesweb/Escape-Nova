using Code.Scripts.Core.Entity;
using Code.Scripts.Core.Systems.Resources;

namespace Code.Scripts.UI.Menus.Trading
{
    public static class TradeOfferCalculator
    {
        public static int CalculateTotalOfferValue(TradingData tradingData, Entity entity)
        {
            int totalValue = 0;
            
            totalValue +=  (int) (tradingData.ResourceData[ResourceType.Batee] * entity.ItemPreferences.stoneWorth);
            totalValue +=  (int) (tradingData.ResourceData[ResourceType.Paladium] * entity.ItemPreferences.metalWorth);
            totalValue +=  (int) (tradingData.ResourceData[ResourceType.Magmavite] * entity.ItemPreferences.fireWorth);
            totalValue +=  (int) (tradingData.ResourceData[ResourceType.Frostice] * entity.ItemPreferences.iceWorth);
            totalValue +=  (int) (tradingData.ResourceData[ResourceType.Sandit] * entity.ItemPreferences.sandWorth);
            
            foreach (var item in tradingData.itemsToTrade)
            {
                if (entity.ItemPreferences.tier0Items.Contains(item))
                {
                    totalValue -= 10; 
                }
                else if (entity.ItemPreferences.tier2Items.Contains(item))
                {
                    totalValue += 20; 
                }
                else if (entity.ItemPreferences.tier3Items.Contains(item))
                {
                    totalValue += 30; 
                }
                else if (entity.ItemPreferences.tier4Items.Contains(item))
                {
                    totalValue += 40; 
                }
            }
            return totalValue;
        }
    }
}