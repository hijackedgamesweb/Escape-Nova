using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class SateliteListPrefab : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _planetSprite;
        [SerializeField] private TMP_Text _planetName;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private ResourceCostInitializer _resourceCostInitializer;
        SateliteListInitializer _parentInitializer;
        public bool IsSelected { get; set; }
        public SateliteDataSO SateliteData { get; set; }
        public void Initialize(SateliteDataSO sateliteData, SateliteListInitializer parentInitializer)
        {
            SateliteData = sateliteData;
            _parentInitializer = parentInitializer;
            _planetSprite.sprite = sateliteData.sprite;
            _planetName.text = sateliteData.constructibleName;
            _descriptionText.text = sateliteData.desc;
            _resourceCostInitializer.Initialize(sateliteData);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            _parentInitializer.SetCurrentSateliteItem(this);
        }
        
        public void UpdateVisualState()
        {
            if (IsSelected)
            {
                _planetSprite.color = Color.yellow;
            }
            else
            {
                _planetSprite.color = Color.white;
            }
        }
    }
}