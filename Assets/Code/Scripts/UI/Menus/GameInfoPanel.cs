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
        [SerializeField] private Button _nextPageButton;
        [SerializeField] private Button _previousPageButton;

        private String[] _currentTitle;
        private String[] _currentMessage;
        private int _currentPage = 1;
        private int _maxPages;
        
        
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
            _nextPageButton.onClick.AddListener(delegate{SwapPages(1);});
            _previousPageButton.onClick.AddListener(delegate{SwapPages(-1);});
            
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
        
        
        public void ShowPanel(String[] titles, String[] messages)
        {
            gameObject.SetActive(true);

            _currentTitle = titles;
            _currentMessage = messages;

            _currentPage = 1;
            _maxPages = _currentMessage.Length;
            
            if (_maxPages != 1) { _nextPageButton.interactable = true; }
            else
            {
                _nextPageButton.interactable = false;
                _previousPageButton.interactable = false;
            }
            
            UpdateInfo();
        }
        
        
        private void SwapPages(int dir)
        {
            _currentPage += dir;

            UpdateInfo();
            
            _previousPageButton.interactable = _currentPage != 1;
            _nextPageButton.interactable = _currentPage != _maxPages;
        }
        
        
        private void UpdateInfo()
        {
            _messageTitle.text = _currentTitle[_currentPage-1];
            _messageText.text = _currentMessage[_currentPage-1];
        }
        
        
        private void OnCloseButtonClicked()
        {
            AudioManager.Instance.PlaySFX("Close");
            gameObject.SetActive(false);
        }
    }
}