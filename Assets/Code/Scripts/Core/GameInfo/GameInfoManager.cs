using System;
using Code.Scripts.Core.GameInfo;
using Code.Scripts.UI.Menus;
using UnityEngine;

public class GameInfoManager : MonoBehaviour
{
    // Variables
    public static GameInfoManager Instance { get; private set; }

    [SerializeField] private GameInfoPanel _gameInfoPanel;
    [SerializeField] private GameInfoSO[] gameInfos;
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
        if(_gameInfoPanel != null) _gameInfoPanel.gameObject.SetActive(false);
    }
    
    public void DisplayGameInfo(String name)
    {
        GameInfoSO infoToDisplay = null; 
        
        foreach (var gameInfo in gameInfos)
        {
            if (gameInfo == null) continue;

            if (gameInfo.name == name)
            {
                infoToDisplay = gameInfo;
                break;
            }
        }
        if (infoToDisplay != null)
        {
            _gameInfoPanel.ShowPanel(infoToDisplay.titles, infoToDisplay.messages, infoToDisplay.images);
        }
        else
        {
        }
    }
}