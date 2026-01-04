using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;
using Code.Scripts.Core.World;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Code.Scripts.Core.Managers
{
    public class WarUIManager : MonoBehaviour
    {
        [Header("Paneles Principales")]
        [SerializeField] private GameObject proposalPanel;      // Panel de "Aceptar con 'A' / Rechazar"
        [SerializeField] private GameObject consequencePanel;   // Panel de "Has perdido recursos"
        [SerializeField] private GameObject battlePanel;        // Panel de la Guerra activa

        [Header("Battle UI Elements")]
        [SerializeField] private GameObject noAmmoPanel;        
        [SerializeField] private Transform logContent;          
        [SerializeField] private GameObject logTextPrefab;      
        
        [Header("Configuración")]
        [SerializeField] private int playerDamageAmount = 25;

        private Civilization _currentAggressor;
        private BaseBehaviour _activeBehaviour;

        private void OnEnable()
        {
            BaseBehaviour.OnWarDeclaredToPlayer += ShowProposal;
            BaseBehaviour.OnPeaceSigned += HandlePeaceSigned;
        }

        private void OnDisable()
        {
            BaseBehaviour.OnWarDeclaredToPlayer -= ShowProposal;
            BaseBehaviour.OnPeaceSigned -= HandlePeaceSigned;
            
            if (_activeBehaviour != null)
                _activeBehaviour.OnBattleLog -= AddLogToPanel;
        }

        // --- NUEVO: INPUT POR TECLADO ---
        private void Update()
        {
            // Solo funciona si el panel de propuesta está abierto
            if (proposalPanel.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    OnAcceptWar();
                }
            }
        }

        private void ShowProposal(Civilization aggressor)
        {
            _currentAggressor = aggressor;
            proposalPanel.SetActive(true);
            consequencePanel.SetActive(false);
            battlePanel.SetActive(false);
        }

        public void OnAcceptWar()
        {

            if (_currentAggressor != null)
            {
                var behaviour = _currentAggressor.AIController as BaseBehaviour;
                if (behaviour != null)
                {
                    _activeBehaviour = behaviour;
                    
                    proposalPanel.SetActive(false);
                    battlePanel.SetActive(true);
                    _activeBehaviour.OnBattleLog += AddLogToPanel;
                    behaviour.StartWar();
                    
                    AddLogToPanel($"<color=green>JUGADOR: War accepted. ¡To weapons against {_currentAggressor.CivilizationData.Name}!</color>");
                } 
            }
        }

        public void OnDeclineWar()
        {
            Debug.Log("PLAYER: War rejected.");
            proposalPanel.SetActive(false);
            consequencePanel.SetActive(true);
        }

        public void OnPlayerFireStrikeButton()
        {
            var playerStorage = WorldManager.Instance.Player.StorageSystem;
            if (playerStorage == null) return;

            string itemName = "Fire Strike"; 
            if (playerStorage.GetItemCount(itemName) > 0)
            {
                playerStorage.ConsumeInventoryItem(itemName, 1);
                
                AddLogToPanel($"<color=green><b>[TÚ]</b> ¡Successful Launch! -1 {itemName}</color>");
                
                if (_activeBehaviour != null)
                {
                    _activeBehaviour.TakeDamageFromPlayer(playerDamageAmount);
                }
            }
            else
            {
                noAmmoPanel.SetActive(true);
            }
        }

        private void AddLogToPanel(string message)
        {
            // Verificamos si el panel está activo para evitar errores
            if (battlePanel.activeSelf && logTextPrefab != null && logContent != null)
            {
                GameObject newLog = Instantiate(logTextPrefab, logContent);
                newLog.transform.localScale = Vector3.one; // Asegurar escala
                
                var tmp = newLog.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null)
                {
                    tmp.text = message;
                }
                
                Canvas.ForceUpdateCanvases();
                logContent.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0f;
            }
        }

        public void OnCloseNoAmmoPanel()
        {
            noAmmoPanel.SetActive(false);
        }

        public void OnCloseConsequences()
        {
            consequencePanel.SetActive(false);
        }

        private void HandlePeaceSigned(Civilization civ)
        {
            battlePanel.SetActive(false);
            
            if (_activeBehaviour != null)
            {
                _activeBehaviour.OnBattleLog -= AddLogToPanel;
                _activeBehaviour = null;
            }
        }
    }
}