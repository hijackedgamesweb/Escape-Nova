using Code.Scripts.Core.SceneManagement;
using UnityEngine;

public class MainMenuButtonsController : MonoBehaviour
{
    public void OnPlayButtonPressed()
    {
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
        Debug.Log("Continue button pressed");
    }
}
