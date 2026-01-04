using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;
using Code.Scripts.Core.World; // Necesario para acceder al Player
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.World
{
    public class WarStatusPanel : MonoBehaviour
    {
        [Header("Enemy Status")]
        [SerializeField] private TextMeshProUGUI enemyNameText;
        [SerializeField] private Slider enemyHealthSlider;
        [SerializeField] private TextMeshProUGUI enemyHealthText;

        [Header("Player Status")]
        [SerializeField] private Slider playerHealthSlider;
        [SerializeField] private TextMeshProUGUI playerHealthText;
        [SerializeField] private TextMeshProUGUI playerAmmoText;

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

        private void Update()
        {
            if (panelContainer.activeSelf && playerAmmoText != null)
            {
                UpdatePlayerAmmoText();
            }
        }

        private void UpdatePlayerAmmoText()
        {
            if (WorldManager.Instance != null && WorldManager.Instance.Player != null)
            {
                int ammo = WorldManager.Instance.Player.StorageSystem.GetItemCount("Fire Strike");
                playerAmmoText.text = $"Fire Strikes: {ammo}";
            }
        }

        private void ShowPanel(Code.Scripts.Core.Entity.Civilization.Civilization civ)
        {
            if(panelContainer != null) panelContainer.SetActive(true);
            if(enemyNameText != null) enemyNameText.text = civ.CivilizationData.Name;
        
            UpdateUI(100, 100);
        }

        private void HidePanel(Code.Scripts.Core.Entity.Civilization.Civilization civ)
        {
            if(panelContainer != null) panelContainer.SetActive(false);
        }

        private void UpdateUI(int enemyHealth, int playerHealth)
        {
            if (enemyHealthSlider != null)
            {
                enemyHealthSlider.maxValue = 100;
                enemyHealthSlider.value = enemyHealth;
            }
            if (enemyHealthText != null) enemyHealthText.text = $"{enemyHealth}%";

            if (playerHealthSlider != null)
            {
                playerHealthSlider.maxValue = 100;
                playerHealthSlider.value = playerHealth;
            }
            if (playerHealthText != null) playerHealthText.text = $"{playerHealth}%";
        }
    }
}