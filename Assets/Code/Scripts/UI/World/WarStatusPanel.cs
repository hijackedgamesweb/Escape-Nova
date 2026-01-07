using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;
using Code.Scripts.Core.World;
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
            SystemEvents.OnWarHealthUpdated += UpdateUI;
        }

        private void OnDestroy()
        {
            SystemEvents.OnWarHealthUpdated -= UpdateUI;
        }

        private void Update()
        {
            if ((panelContainer != null && panelContainer.activeSelf) || gameObject.activeInHierarchy)
            {
                if (playerAmmoText != null) UpdatePlayerAmmoText();
            }
        }

        public void SetupBattle(string enemyName, int currentEnemyHealth, int currentPlayerHealth)
        {
            if (enemyNameText != null) 
                enemyNameText.text = enemyName;
            UpdateUI(currentEnemyHealth, currentPlayerHealth);
        }

        private void UpdatePlayerAmmoText()
        {
            if (WorldManager.Instance != null && WorldManager.Instance.Player != null)
            {
                int ammo = WorldManager.Instance.Player.StorageSystem.GetItemCount("Fire Strike");
                playerAmmoText.text = $"Fire Strikes: {ammo}";
            }
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