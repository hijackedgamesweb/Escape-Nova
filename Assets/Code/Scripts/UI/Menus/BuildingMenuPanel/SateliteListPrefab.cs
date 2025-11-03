using Code.Scripts.Core.World.ConstructableEntities.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class SateliteListPrefab : MonoBehaviour
    {
        [SerializeField] private Image _planetSprite;
        [SerializeField] private TMP_Text _planetName;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private ResourceCostInitializer _resourceCostInitializer;

        public void Initialize(SateliteDataSO sateliteData)
        {
            _planetSprite.sprite = sateliteData.sprite;
            _planetName.text = sateliteData.constructibleName;
            _descriptionText.text = sateliteData.desc;
            _resourceCostInitializer.Initialize(sateliteData);
        }
    }
}