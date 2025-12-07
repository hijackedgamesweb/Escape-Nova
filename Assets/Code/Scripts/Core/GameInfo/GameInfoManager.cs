using System;
using System.Collections.Generic;
using Code.Scripts.UI.Menus;
using UnityEngine;

namespace Code.Scripts.Core.GameInfo
{
    public class GameInfoManager : MonoBehaviour
    {
        public static GameInfoManager Instance { get; private set; }

        [SerializeField] private GameInfoPanel _gameInfoPanel;
        [SerializeField] private GameInfoSO[] gameInfos;
        private Dictionary<string, GameInfoSO> _infoDictionary = new Dictionary<string, GameInfoSO>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            foreach (var info in gameInfos)
            {
                if (!_infoDictionary.ContainsKey(info.name))
                {
                    _infoDictionary.Add(info.name, info);
                }
            }

            gameObject.SetActive(false); 
        }
        
        public void DisplayGameInfo(GameInfoSO infoConfig)
        {
            if (infoConfig == null) 
            {
                Debug.LogWarning("GameInfoManager: Se intentó mostrar información pero el SO es nulo.");
                return;
            }

            gameObject.SetActive(true); 
            _gameInfoPanel.ShowPanel(infoConfig.titles, infoConfig.messages, infoConfig.images);
        }

        public void DisplayGameInfo(string name)
        {
            if (_infoDictionary.TryGetValue(name, out GameInfoSO foundInfo))
            {
                DisplayGameInfo(foundInfo);
            }
            else
            {
                Debug.LogWarning($"GameInfoManager: No se encontró información con el nombre '{name}'");
            }
        }
    }
}