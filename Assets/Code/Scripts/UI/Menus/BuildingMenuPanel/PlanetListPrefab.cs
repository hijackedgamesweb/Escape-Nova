using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class PlanetListPrefab : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _planetSprite;
        [SerializeField] private TMP_Text _planetName;
        [SerializeField] private TMP_Text _planetProductionResource;
        [SerializeField] private ResourceCostInitializer _resourceCostInitializer;

        PlanetListInitializer _parentInitializer;
        public bool IsSelected { get; set; }
        public PlanetDataSO PlanetData { get; set; }

        public void Initialize(PlanetDataSO planetData, PlanetListInitializer parentInitializer)
        {
            PlanetData = planetData;
            _parentInitializer = parentInitializer;
            _planetSprite.sprite = planetData.sprite;
            _planetName.text = planetData.constructibleName;
            
            var production = "";
            for (int i = 0; i < planetData.producibleResources.Count; i++)
            {
                production += $"{planetData.resourcePerCycle[i]} {planetData.producibleResources[i].DisplayName} / cycle";
                if (i < planetData.producibleResources.Count - 1)
                    production += ", \n";
            }
            _planetProductionResource.text = production;
            _resourceCostInitializer.Initialize(planetData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _parentInitializer.SetCurrentPlanetItem(this);
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