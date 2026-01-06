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
            
            if (_currentPlanet.ProducibleResources != null && _currentPlanet.ResourcePerCycle != null)
            {
                // Iteramos solo sobre lo que el planeta tiene definido
                for (int i = 0; i < _currentPlanet.ProducibleResources.Count; i++)
                {
                    // Aseguramos que no nos salimos del array de cantidades
                    if (i < _currentPlanet.ResourcePerCycle.Length)
                    {
                        ResourceType type = _currentPlanet.ProducibleResources[i];
                        int amount = _currentPlanet.ResourcePerCycle[i];

                        // Si quisieramso ver los que no producen nada, habríua que quitar este if
                        if (amount > 0)
                        {
                            productionString.AppendLine($"{type}: {amount}");
                        }
                    }
                }
            }
            
            // Si el planeta no tiene recursos configurados, mostramos un mensaje por defecto
            if (productionString.Length <= "Production per cicle\n".Length) 
            {
                productionString.AppendLine("None");
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
            if (UnityEngine.Camera.main != null)
            {
                var camController = UnityEngine.Camera.main.GetComponent<Code.Scripts.Camera.CameraController2D>();
                if (camController != null) 
                    camController.ClearTarget();
            }
            
            AudioManager.Instance.PlaySFX("Close");
            HidePanel();
        }
        
        private void OnDeletePlanetClicked()
        {
            if (_currentPlanet == null) return;
            if (!_currentPlanet.CanBeDestroyedByPlayer(out string reason))
            {
                NotificationManager.Instance.CreateNotification(reason, NotificationType.Error);
                Debug.Log($"<color=yellow>[UI] Deleting try blocked: {reason}</color>");
                return; 
            }

            _currentPlanet.DestroyPlanet();
            OnCloseButtonClicked();
        }
    }
}