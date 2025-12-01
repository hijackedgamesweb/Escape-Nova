using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.Systems.Storage;
using TMPro;
using UnityEngine;

namespace Code.Scripts.UI.Menus.Trading
{
    public class TradingPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text stoneAmountText;
        [SerializeField] private TMP_Text metalAmountText;
        [SerializeField] private TMP_Text fireAmountText;
        [SerializeField] private TMP_Text iceAmountText;
        [SerializeField] private TMP_Text sandAmountText;
        
        [SerializeField] private TradingObjectsGrid tradingObjectsGrid;
        private StorageSystem _storage;
        
        public void InitializePanel(StorageSystem storage)
        {
            _storage = storage;
            stoneAmountText.text = "0";
            metalAmountText.text = "0";
            fireAmountText.text = "0";
            iceAmountText.text = "0";
            sandAmountText.text = "0";
            tradingObjectsGrid.InitializeGrid(_storage.GetInventoryItemList());
        }
        
        public TradingData GetTradingData()
        {
            int stoneAmount = int.Parse(stoneAmountText.text);
            int metalAmount = int.Parse(metalAmountText.text);
            int fireAmount = int.Parse(fireAmountText.text);
            int iceAmount = int.Parse(iceAmountText.text);
            int sandAmount = int.Parse(sandAmountText.text);
            var itemsToTrade = tradingObjectsGrid.GetInventoryItems();
            return new TradingData(stoneAmount, metalAmount, fireAmount, iceAmount, sandAmount, itemsToTrade);
        }
        
        // TODO: Refactorizar el add y remove resources
        // NO HAGAIS ESTO ASI POR FAVOR, ES SOLO PARA LA ENTREGA DE COMPORTAMIENTOS 
        public void AddResource(string resourceType)
        {
            switch (resourceType)
            {
                case "Stone":
                    int stoneAmount = int.Parse(stoneAmountText.text);
                    if (stoneAmount + 10 <= _storage.GetResourceAmount(ResourceType.Stone))
                    {
                        stoneAmount += 10;
                        stoneAmountText.text = stoneAmount.ToString();
                    }
                    else
                    {
                        stoneAmount = _storage.GetResourceAmount(ResourceType.Stone);
                        stoneAmountText.text = stoneAmount.ToString();
                    }
                    break;
                case "Metal":
                    int metalAmount = int.Parse(metalAmountText.text);
                    if (metalAmount + 10 <= _storage.GetResourceAmount(ResourceType.Metal))
                    {
                        metalAmount += 10;
                        metalAmountText.text = metalAmount.ToString();
                    }
                    else
                    {
                        metalAmount = _storage.GetResourceAmount(ResourceType.Metal);
                        metalAmountText.text = metalAmount.ToString();
                    }
                    break;
                case "Fire":
                    int fireAmount = int.Parse(fireAmountText.text);
                    if (fireAmount + 10 <= _storage.GetResourceAmount(ResourceType.Fire))
                    {
                        fireAmount += 10;
                        fireAmountText.text = fireAmount.ToString();
                    }
                    else
                    {
                        fireAmount = _storage.GetResourceAmount(ResourceType.Fire);
                        fireAmountText.text = fireAmount.ToString();
                    }
                    break;
                case "Ice":
                    int iceAmount = int.Parse(iceAmountText.text);
                    if (iceAmount + 10 <= _storage.GetResourceAmount(ResourceType.Ice))
                    {
                        iceAmount += 10;
                        iceAmountText.text = iceAmount.ToString();
                    }
                    else
                    {
                        iceAmount = _storage.GetResourceAmount(ResourceType.Ice);
                        iceAmountText.text = iceAmount.ToString();
                    }
                    break;
                case "Sand":
                    int sandAmount = int.Parse(sandAmountText.text);
                    if (sandAmount + 10 <= _storage.GetResourceAmount(ResourceType.Sand))
                    {
                        sandAmount += 10;
                        sandAmountText.text = sandAmount.ToString();
                    }
                    else
                    {
                        sandAmount = _storage.GetResourceAmount(ResourceType.Sand);
                        sandAmountText.text = sandAmount.ToString();
                    }
                    break;
            }
        }
        
        public void RemoveResource(string resourceType)
        {
            switch (resourceType)
            {
                case "Stone":
                    int stoneAmount = int.Parse(stoneAmountText.text);
                    if (stoneAmount - 10 >= 0)
                    {
                        stoneAmount -= 10;
                        stoneAmountText.text = stoneAmount.ToString();
                    }
                    else
                    {
                        stoneAmount = 0;
                        stoneAmountText.text = stoneAmount.ToString();
                    }
                    break;
                case "Metal":
                    int metalAmount = int.Parse(metalAmountText.text);
                    if (metalAmount - 10 >= 0)
                    {
                        metalAmount -= 10;
                        metalAmountText.text = metalAmount.ToString();
                    }
                    else
                    {
                        metalAmount = 0;
                        metalAmountText.text = metalAmount.ToString();
                    }
                    break;
                case "Fire":
                    int fireAmount = int.Parse(fireAmountText.text);
                    if (fireAmount - 10 >= 0)
                    {
                        fireAmount -= 10;
                        fireAmountText.text = fireAmount.ToString();
                    }
                    else
                    {
                        fireAmount = 0;
                        fireAmountText.text = fireAmount.ToString();
                    }
                    break;
                case "Ice":
                    int iceAmount = int.Parse(iceAmountText.text);
                    if (iceAmount - 10 >= 0)
                    {
                        iceAmount -= 10;
                        iceAmountText.text = iceAmount.ToString();
                    }
                    else
                    {
                        iceAmount = 0;
                        iceAmountText.text = iceAmount.ToString();
                    }
                    break;
                case "Sand":
                    int sandAmount = int.Parse(sandAmountText.text);
                    if (sandAmount - 10 >= 0)
                    {
                        sandAmount -= 10;
                        sandAmountText.text = sandAmount.ToString();
                    }
                    else
                    {
                        sandAmount = 0;
                        sandAmountText.text = sandAmount.ToString();
                    }
                    break;
            }
        }
    }
}