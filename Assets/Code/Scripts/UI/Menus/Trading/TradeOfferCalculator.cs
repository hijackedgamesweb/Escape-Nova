using Code.Scripts.Core.Entity;

namespace Code.Scripts.UI.Menus.Trading
{
    public static class TradeOfferCalculator
    {
        public static int CalculateTotalOfferValue(TradingData tradingData, Entity entity)
        {
            int totalValue = 0;
            
            totalValue +=  (int) (tradingData.stoneAmount * entity.ItemPreferences.stoneWorth);
            totalValue +=  (int) (tradingData.metalAmount * entity.ItemPreferences.metalWorth);
            totalValue +=  (int) (tradingData.fireAmount * entity.ItemPreferences.fireWorth);
            totalValue +=  (int) (tradingData.iceAmount * entity.ItemPreferences.iceWorth);
            totalValue +=  (int) (tradingData.sandAmount * entity.ItemPreferences.sandWorth);
            
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