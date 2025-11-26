using Code.Scripts.Core.Managers;
using UnityEngine.InputSystem;

namespace Code.Scripts.UI.Windows
{
    public class MissionScreen : BaseUIScreen
    {
        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            {
                UIManager.Instance.ShowScreen<InGameScreen>();
            }
        }
    }
}