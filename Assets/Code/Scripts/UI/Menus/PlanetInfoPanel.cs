using System;
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
    public class PlanetInfoPanel : MonoBehaviour
    {
        public static PlanetInfoPanel Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _planetNameText;
        [SerializeField] private TextMeshProUGUI _productionText;
        [SerializeField] private Transform _satelliteListContainer;
        [SerializeField] private GameObject _satelliteListItemPrefab;
        [SerializeField] private Button _deletePlanetButton;
        [SerializeField] private Button _closeButton;
        
        private Planet _currentPlanet;
        
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
            
            gameObject.SetActive(false);
        }

        private void Start()
        {
            _deletePlanetButton.onClick.AddListener(OnDeletePlanetClicked);
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
            
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
        
        public void ShowPanel(Planet planet)
        {
            _currentPlanet = planet;
            if (_currentPlanet == null)
            {
                return;
            }
            
            gameObject.SetActive(true);
            
            _planetNameText.text = _currentPlanet.Name;
            
            StringBuilder productionString = new StringBuilder("Production per cicle\n");
            
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                int resourceIndex = (int)type;
                if (resourceIndex >= 0 && resourceIndex < _currentPlanet.ResourcePerCycle.Length)
                {
                    int amount = _currentPlanet.ResourcePerCycle[resourceIndex];
                    productionString.AppendLine($"{type.ToString()}: {amount}");
                }
                else
                {
                    productionString.AppendLine($"{type.ToString()}: 0");
                }
            }
            _productionText.text = productionString.ToString();

            
            foreach (Transform child in _satelliteListContainer)
            {
                Destroy(child.gameObject);
            }
            
            if (_currentPlanet.Satelites.Count > 0)
            {
                foreach (Satelite satelite in _currentPlanet.Satelites)
                {
                    GameObject itemGO = Instantiate(_satelliteListItemPrefab, _satelliteListContainer);
                    itemGO.GetComponentInChildren<TextMeshProUGUI>().text = satelite.Name;
                }
            }
            else
            {
                GameObject itemGO = Instantiate(_satelliteListItemPrefab, _satelliteListContainer);
                itemGO.GetComponentInChildren<TextMeshProUGUI>().text = "<i>No satellites</i>";
            }
        }

        public void HidePanel()
        {
            gameObject.SetActive(false);
            _currentPlanet = null;
        }

        private void OnCloseButtonClicked()
        {
            UnityEngine.Camera.main.GetComponent<Camera.CameraController2D>().ClearTarget();
            AudioManager.Instance.PlaySFX("Close");
            HidePanel();
        }
        
        private void OnDeletePlanetClicked()
        {
            if (_currentPlanet == null) return;

            SolarSystem solarSystem = ServiceLocator.GetService<SolarSystem>();
            if (solarSystem != null)
            {
                solarSystem.RemovePlanet(_currentPlanet.OrbitIndex, _currentPlanet.PlanetIndex);
            }
            
            OnCloseButtonClicked();
        }
    }
}