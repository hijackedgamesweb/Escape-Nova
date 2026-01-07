using System;
using System.Collections.Generic;
using System.Text;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.Core.World;
using Code.Scripts.Core.World.ConstructableEntities;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.UI.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus
{
    public class SunInfoPanel : MonoBehaviour
    {
        public static SunInfoPanel Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _totalProductionText;
        [SerializeField] private Button _closeButton;

        private SolarSystem _solarSystem;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            gameObject.SetActive(false); 
            
            _solarSystem = ServiceLocator.GetService<SolarSystem>();
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(OnCloseButtonClicked);
            }
            
            UIManager.OnScreenChanged += HandleScreenChange;
        }
        
        private void OnDestroy()
        {
            UIManager.OnScreenChanged -= HandleScreenChange;
        }

        private void HandleScreenChange()
        {
            if (gameObject.activeSelf && UIManager.Instance.GetCurrentScreen() is not InGameScreen)
            {
                OnCloseButtonClicked();
            }
        }

        public void ShowPanel()
        {
            if (_solarSystem == null) return;
            
            gameObject.SetActive(true);
            CalculateAndDisplayTotalProduction();
        }

        private void CalculateAndDisplayTotalProduction()
        {
            Dictionary<ResourceType, int> totalProduction = new Dictionary<ResourceType, int>();
        
            foreach (var orbit in _solarSystem.Planets)
            {
                foreach (var planet in orbit)
                {
                    if (planet != null && planet.ProducibleResources != null)
                    {
                        if (planet.IsConquered) 
                        {
                            continue; 
                        }
        
                        for (int i = 0; i < planet.ProducibleResources.Count; i++)
                        {
                            if (i < planet.ResourcePerCycle.Length)
                            {
                                ResourceType type = planet.ProducibleResources[i];
                                int amount = planet.ResourcePerCycle[i];
        
                                if (!totalProduction.ContainsKey(type))
                                {
                                    totalProduction[type] = 0;
                                }
                                totalProduction[type] += amount;
                            }
                        }
                    }
                }
            }
        
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<b>Total System Production</b>\n");
            
            bool hasActiveProduction = false;
            foreach (var kvp in totalProduction)
            {
                if (kvp.Value > 0)
                {
                    hasActiveProduction = true;
                    break;
                }
            }
        
            if (!hasActiveProduction)
            {
                sb.AppendLine("No production active.");
            }
            else
            {
                foreach (var kvp in totalProduction)
                {
                    if (kvp.Value > 0)
                    {
                        sb.AppendLine($"{kvp.Key}: <color=green>+{kvp.Value}</color> / cycle");
                    }
                }
            }
        
            if (_totalProductionText != null)
                _totalProductionText.text = sb.ToString();
        }

        public void HidePanel()
        {
            gameObject.SetActive(false);
        }

        private void OnCloseButtonClicked()
        {
            if (UnityEngine.Camera.main != null)
            {
                var camController = UnityEngine.Camera.main.GetComponent<Code.Scripts.Camera.CameraController2D>();
                if (camController != null) 
                    camController.ClearTarget();
            }
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("Close");
                
            HidePanel();
        }
    }
}