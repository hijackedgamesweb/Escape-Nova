using System;
using Code.Scripts.Core.Events;
using Code.Scripts.Core.SaveLoad;
using Code.Scripts.Core.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtonsController : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject storePanel;
    private void Start()
    {
        bool hasSaveData = SaveManager.Instance.SlotExists();
        continueButton.interactable = hasSaveData;  
    }

    public void OnPlayButtonPressed()
    {
        AudioManager.Instance.PlaySFX("MainMenuButtonPressed");
        AudioManager.Instance.PlaySFX("Spaceship");
        SaveManager.Instance.DeleteSlot();
        SystemEvents.LockAll();
        Loader.Load("InGame");
    }
    
    public void OnExitButtonPressed()
    {
        AudioManager.Instance.PlaySFX("MainMenuButtonPressed");
        Debug.Log("Exit button pressed");
        Application.Quit();
    }
    
    public void OnCreditsButtonPressed()
    {
        AudioManager.Instance.PlaySFX("MainMenuButtonPressed");
        Loader.Load("CreditsScene");
    }
    
    public void OnLoadGameButtonPressed()
    {
        AudioManager.Instance.PlaySFX("MainMenuButtonPressed");
        Debug.Log("Load Game button pressed");
    }
    
    public void OnContinueButtonPressed()
    {
        AudioManager.Instance.PlaySFX("MainMenuButtonPressed");
        Loader.Load("InGame");
    }
    
    public void OnStoreButtonPressed()
    {
        AudioManager.Instance.PlaySFX("MainMenuButtonPressed");
        storePanel.SetActive(true);
    }

    public void OnStoreExitButtonPressed()
    {
        AudioManager.Instance.PlaySFX("Close");
    }
}