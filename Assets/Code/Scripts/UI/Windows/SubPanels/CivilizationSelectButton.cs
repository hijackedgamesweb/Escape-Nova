using Code.Scripts.Core.Entity.Civilization;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Windows.SubPanels
{
    public class CivilizationSelectButton : MonoBehaviour
    {
        [SerializeField] Image _civilizationImage;
        [SerializeField] Button _button;

        public void Initialize(CivilizationData civilizationCivilizationData)
        {
            _civilizationImage.sprite = civilizationCivilizationData.CivilizationIcon;
            var civScreen = GetComponentInParent<CivilizationScreen>();
            Debug.Log($"Initialize for {civilizationCivilizationData.Name}: civScreen = {civScreen}");
            _button.onClick.AddListener(() => 
                civScreen.SetCivilization(civilizationCivilizationData.Name));
        }
    }
}