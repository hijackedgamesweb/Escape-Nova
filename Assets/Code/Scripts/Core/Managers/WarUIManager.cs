using System.Collections;
using Code.Scripts.Config;
using Code.Scripts.Core.Entity.Civilization;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Diplomacy.AI.Behaviour.USBehaviour;
using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.World;
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
        [SerializeField] private GameObject acceptButton;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private TextMeshProUGUI cooldownText;
        [SerializeField] private float attackCooldownDuration = 8f;

        [Header("Configuración")]
        [SerializeField] private int playerDamageAmount = 25;
        
        [Header("Skill Check System")]
        [SerializeField] private AttackSkillCheckUI skillCheckSystem;

        private Civilization _currentAggressor;
        private BaseBehaviour _activeBehaviour;
        private bool _isCooldownActive = false;

        private IGameTime _gameTime;
        private TimeConfig _timeConfig;

        private void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
            _timeConfig = ServiceLocator.GetService<TimeConfig>();
        }

        private void OnEnable()
        {
            SystemEvents.OnWarDeclaredToPlayer += ShowProposal;
            SystemEvents.OnPeaceSigned += HandlePeaceSigned;
            
            ResetCooldownUI();
        }

        private void OnDisable()
        {
            SystemEvents.OnWarDeclaredToPlayer -= ShowProposal;
            SystemEvents.OnPeaceSigned -= HandlePeaceSigned;
            
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
            acceptButton.SetActive(true);
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
                    acceptButton.SetActive(false);
                    AddLogToPanel($"<color=yellow>War started against {_currentAggressor.CivilizationData.Name}!</color>");
                } 
            }
        }

        public void OpenBattlePanelForPlanet(Planet planet, BaseBehaviour behaviour)
        {
            battlePanel.SetActive(true);
            proposalPanel.SetActive(false);
            _activeBehaviour = behaviour;
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
            
            if (_activeBehaviour == null || !_activeBehaviour._isAtWarWithPlayer) return;

            if (_activeBehaviour.WarHealth <= 0) return;

            var playerStorage = WorldManager.Instance.Player.StorageSystem;
            if (playerStorage == null) return;

            string itemName = "Fire Strike"; 
            if (playerStorage.GetItemCount(itemName) > 0)
            {
                skillCheckSystem.StartSkillCheck(HandleAttackResult);
            }
            else
            {
                noAmmoPanel.SetActive(true);
            }
        }

        private void HandleAttackResult(AttackSkillCheckUI.SkillCheckResult result)
        {
            var playerStorage = WorldManager.Instance.Player.StorageSystem;
            playerStorage.ConsumeInventoryItem("Fire Strike", 1);
            
            StartCoroutine(CooldownRoutine());
            
            switch (result)
            {
                case AttackSkillCheckUI.SkillCheckResult.Miss:
                    AddLogToPanel($"<color=yellow><b>[FAILURE]</b> Missile wasted! [0 damage]</color>");
                    break;
                case AttackSkillCheckUI.SkillCheckResult.Chance50:
                    bool isHit = UnityEngine.Random.value > 0.5f;
                    if (isHit) {
                        _activeBehaviour.TakeDamageFromPlayer(playerDamageAmount);
                        AddLogToPanel($"<color=yellow><b>[BORDERING]</b> Unstable impact! [Damage applied]</color>");
                    } else {
                        AddLogToPanel($"<color=yellow><b>[BORDERING]</b> The missile missed.</color>");
                    }
                    break;
                case AttackSkillCheckUI.SkillCheckResult.Hit100:
                    _activeBehaviour.TakeDamageFromPlayer(playerDamageAmount);
                    AddLogToPanel($"<color=green><b>[PERFECT]</b> Critical impact confirmed!</color>");
                    break;
            }
        }

        private IEnumerator CooldownRoutine()
        {
            _isCooldownActive = true;
            if (fireButton != null) fireButton.interactable = false;

            float secondsPerCycle = _timeConfig.secondsPerCycle;
            
            float startTime = _gameTime.GameTime;
            float duration = attackCooldownDuration;
            float endTime = startTime + duration;

            while (_gameTime.GameTime < endTime)
            {
                yield return null; 

                float timeRemaining = endTime - _gameTime.GameTime;
                if (cooldownOverlay != null)
                {
                    cooldownOverlay.fillAmount = timeRemaining / duration;
                }
                if (cooldownText != null)
                {
                    int cyclesRemaining = Mathf.CeilToInt(timeRemaining / secondsPerCycle);
                    if (cyclesRemaining < 1 && timeRemaining > 0) cyclesRemaining = 1;
                    
                    cooldownText.text = cyclesRemaining.ToString() + " cycles";
                }
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
                if (tmp != null) tmp.text = message;
                Canvas.ForceUpdateCanvases();
                logContent.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0f;
            }
        }

        public void OnCloseNoAmmoPanel() { noAmmoPanel.SetActive(false); }
        public void OnCloseConsequences() { consequencePanel.SetActive(false); }

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