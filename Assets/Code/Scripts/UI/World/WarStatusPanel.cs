using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Code.Scripts.UI.World
{
    public class WarStatusPanel : MonoBehaviour
    {
        [Header("Enemy Status (The Civilization)")]
        [SerializeField] private TextMeshProUGUI enemyNameText;
        [SerializeField] private Slider enemyHealthSlider;
        [SerializeField] private TextMeshProUGUI enemyHealthText;

        [Header("Player Status (Nosotros)")]
        [SerializeField] private Slider playerHealthSlider;
        [SerializeField] private TextMeshProUGUI playerHealthText;

        [Header("Container")]
        [SerializeField] private GameObject panelContainer;

        private void Awake()
        {
            BaseBehaviour.OnWarHealthUpdated += UpdateUI;
            BaseBehaviour.OnWarDeclaredToPlayer += ShowPanel;
            BaseBehaviour.OnPeaceSigned += HidePanel;
        
            if(panelContainer != null) panelContainer.SetActive(false);
        }

        private void OnDestroy()
        {
            BaseBehaviour.OnWarHealthUpdated -= UpdateUI;
            BaseBehaviour.OnWarDeclaredToPlayer -= ShowPanel;
            BaseBehaviour.OnPeaceSigned -= HidePanel;
        }

        private void ShowPanel(Code.Scripts.Core.Entity.Civilization.Civilization civ)
        {
            if(panelContainer != null) panelContainer.SetActive(true);
            if(enemyNameText != null) enemyNameText.text = civ.CivilizationData.Name;
        
            // Reset visual
            UpdateUI(100, 100);
        }

        private void HidePanel(Code.Scripts.Core.Entity.Civilization.Civilization civ)
        {
            if(panelContainer != null) panelContainer.SetActive(false);
        }

        // Este método se llamará automáticamente cuando la IA cambie las vidas
        private void UpdateUI(int enemyHealth, int playerHealth)
        {
            if (enemyHealthSlider != null) enemyHealthSlider.value = enemyHealth;
            if (enemyHealthText != null) enemyHealthText.text = $"{enemyHealth}%";

            if (playerHealthSlider != null) playerHealthSlider.value = playerHealth;
            if (playerHealthText != null) playerHealthText.text = $"{playerHealth}%";
        }
    }
}