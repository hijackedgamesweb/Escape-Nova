using System;
using Code.Scripts.Core.GameInfo;
using Code.Scripts.UI.Menus;
using UnityEngine;

public class GameInfoManager : MonoBehaviour
{
    
    //Variables
    public static GameInfoManager Instance { get; private set; }

    [SerializeField] private GameInfoPanel _gameInfoPanel;
    [SerializeField] private GameInfoSO[] gameInfos;
    
    private GameInfoSO _gameInfo;
    
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
    
    
    public void DisplayGameInfo(String name)
    {
        bool found = false;
        
        foreach (var gameInfo in gameInfos)
        {
            if (gameInfo.name == name)
            {
                found = true;
                _gameInfo = gameInfo;
            }
        }
        
        if(found){_gameInfoPanel.ShowPanel(_gameInfo.titles, _gameInfo.messages, _gameInfo.images);}
    }
}