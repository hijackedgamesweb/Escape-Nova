using System;
using Code.Scripts.Core.Managers;
using Code.Scripts.UI.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.UI.Menus
{
    public class GameInfoPanel : MonoBehaviour
    {
        //Variables
        public static GameInfoPanel Instance { get; private set; }
        
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _messageTitle;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _closeButton;
        
        
        //Metodos
        
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
            _closeButton.onClick.AddListener(OnCloseButtonClicked); //Que el boton de cerrar cierre la UI
            
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
                gameObject.SetActive(false); //Esconder el panel si ha salido de la escena actual
            }
        }
        
        
        public void ShowPanel(String title, String message)
        {
            gameObject.SetActive(true);
            
            _messageTitle.text = title;
            _messageText.text = message;
        }
        
        private void OnCloseButtonClicked()
        {
            AudioManager.Instance.PlaySFX("Close");
            gameObject.SetActive(false);
        }
    }
}