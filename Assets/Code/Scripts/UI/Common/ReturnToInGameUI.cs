using Code.Scripts.Core.Managers;
using Code.Scripts.UI.Windows;
using UnityEngine;

public class ReturnToInGameUI : MonoBehaviour
{
   public void ExitConstructionPanel()
   {
      UIManager.Instance.ShowScreen<InGameScreen>();
   }
}
