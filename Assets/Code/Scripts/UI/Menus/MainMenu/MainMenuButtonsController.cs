using System;
using Code.Scripts.Core.SaveLoad;
using Code.Scripts.Core.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonsController : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject settingsPanel;
    private void Start()
    {
        bool hasSaveData = SaveManager.Instance.SlotExists();
        continueButton.interactable = hasSaveData;  
    }

    public void OnPlayButtonPressed()
    {
        SaveManager.Instance.DeleteSlot();
        Loader.Load("InGame");
    }
    
    public void OnExitButtonPressed()
    {
        Debug.Log("Exit button pressed");
        Application.Quit();
    }
    
    public void OnCreditsButtonPressed()
    {
        Debug.Log("Credits button pressed");
    }
    
    public void OnLoadGameButtonPressed()
    {
        Debug.Log("Load Game button pressed");
    }
    
    public void OnContinueButtonPressed()
    {
        Loader.Load("InGame");
    }
    
    public void OnSettingsButtonPressed()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}
