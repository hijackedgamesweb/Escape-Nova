using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class ResourceCostPrefab : MonoBehaviour
    {
        [SerializeField] private Image _resourceIcon;
        [SerializeField] private TMP_Text _resourceAmount;
        
        public void Initialize(Sprite resourceSprite, int amount)
        {
            _resourceIcon.sprite = resourceSprite;
            _resourceAmount.text = amount.ToString();
        }
        
        public void UpdateCosts(int newAmount)
        {
            _resourceAmount.text = newAmount.ToString();
        }
    }
}