using System.Collections;
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
        [SerializeField] private GameObject proposalPanel;      
        [SerializeField] private GameObject consequencePanel;   
        [SerializeField] private GameObject battlePanel;        

        [Header("Battle UI Elements")]
        [SerializeField] private GameObject noAmmoPanel;        
        [SerializeField] private Transform logContent;          
        [SerializeField] private GameObject logTextPrefab;      
        
        [Header("Cooldown Settings")]
        [SerializeField] private Button fireButton;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private TextMeshProUGUI cooldownText;
        [SerializeField] private float attackCooldownDuration = 8f;

        [Header("Configuración")]
        [SerializeField] private int playerDamageAmount = 25;

        private Civilization _currentAggressor;
        private BaseBehaviour _activeBehaviour;
        private bool _isCooldownActive = false;

        private void OnEnable()
        {
            BaseBehaviour.OnWarDeclaredToPlayer += ShowProposal;
            BaseBehaviour.OnPeaceSigned += HandlePeaceSigned;
            
            ResetCooldownUI();
        }

        private void OnDisable()
        {
            BaseBehaviour.OnWarDeclaredToPlayer -= ShowProposal;
            BaseBehaviour.OnPeaceSigned -= HandlePeaceSigned;
            
            if (_activeBehaviour != null)
                _activeBehaviour.OnBattleLog -= AddLogToPanel;
        }

        private void Update()
        {
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
                    
                    AddLogToPanel($"<color=yellow> War started against {_currentAggressor.CivilizationData.Name}!</color>");
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
            if (_isCooldownActive) return;

            var playerStorage = WorldManager.Instance.Player.StorageSystem;
            if (playerStorage == null) return;

            string itemName = "Fire Strike"; 
            if (playerStorage.GetItemCount(itemName) > 0)
            {
                // Consumir item
                playerStorage.ConsumeInventoryItem(itemName, 1);
                
                AddLogToPanel($"<color=white> ¡Successful Launch!</color>");
                
                if (_activeBehaviour != null)
                {
                    _activeBehaviour.TakeDamageFromPlayer(playerDamageAmount);
                }

                StartCoroutine(CooldownRoutine());
            }
            else
            {
                noAmmoPanel.SetActive(true);
            }
        }

        private IEnumerator CooldownRoutine()
        {
            _isCooldownActive = true;
            if (fireButton != null) fireButton.interactable = false;

            float timer = attackCooldownDuration;

            while (timer > 0)
            {
                timer -= Time.deltaTime;

                if (cooldownOverlay != null)
                {
                    cooldownOverlay.fillAmount = timer / attackCooldownDuration;
                }

                if (cooldownText != null)
                {
                    cooldownText.text = Mathf.CeilToInt(timer).ToString();
                }

                yield return null;
            }

            ResetCooldownUI();
        }

        private void ResetCooldownUI()
        {
            _isCooldownActive = false;
            if (fireButton != null) fireButton.interactable = true;
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0;
            if (cooldownText != null) cooldownText.text = "";
        }

        private void AddLogToPanel(string message)
        {
            if (battlePanel.activeSelf && logTextPrefab != null && logContent != null)
            {
                GameObject newLog = Instantiate(logTextPrefab, logContent);
                newLog.transform.localScale = Vector3.one; 
                
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
            ResetCooldownUI();
            
            if (_activeBehaviour != null)
            {
                _activeBehaviour.OnBattleLog -= AddLogToPanel;
                _activeBehaviour = null;
            }
        }
    }
}