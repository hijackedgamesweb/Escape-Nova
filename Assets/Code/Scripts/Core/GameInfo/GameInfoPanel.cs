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
        
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _messageTitle;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Image _iconImage;
        
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _nextPageButton;
        [SerializeField] private Button _previousPageButton;
        
        private String[] _currentTitles;
        private String[] _currentMessages;
        private Sprite[] _currentImages;
        
        private int _currentPage = 1;
        private int _maxPages;
        
        
        //Metodos
        private void Start()
        {
            gameObject.SetActive(false);
            
            _closeButton.onClick.AddListener(OnCloseButtonClicked); //Que el boton de cerrar cierre la UI
            _nextPageButton.onClick.AddListener(delegate{SwapPages(1);});
            _previousPageButton.onClick.AddListener(delegate{SwapPages(-1);});
            
            //UIManager.OnScreenChanged += HandleScreenChange;
        }
        
        /*
        private void OnDestroy()
        {
            UIManager.OnScreenChanged -= HandleScreenChange;
        }
        */
        
        private void HandleScreenChange()
        {
            if (gameObject.activeSelf && UIManager.Instance.GetCurrentScreen() is not InGameScreen)
            {
                gameObject.SetActive(false); //Esconder el panel si ha salido de la escena actual
            }
        }
        
        
        public void ShowPanel(String[] titles, String[] messages, Sprite[] images)
        {
            gameObject.SetActive(true);

            _currentTitles = titles;
            _currentMessages = messages;
            _currentImages = images;

            _currentPage = 1;
            _maxPages = _currentMessages.Length;

            _nextPageButton.interactable = _maxPages != 1;
            _previousPageButton.interactable = false;
            
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
            _messageTitle.text = _currentTitles[_currentPage-1];
            _messageText.text = _currentMessages[_currentPage-1];
            
            _iconImage.enabled = _currentImages[_currentPage-1] != null;
            _iconImage.sprite = _currentImages[_currentPage-1];
            
            AudioManager.Instance.PlaySFX("ButtonClick");
        }
        
        
        private void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
        }
    }
}