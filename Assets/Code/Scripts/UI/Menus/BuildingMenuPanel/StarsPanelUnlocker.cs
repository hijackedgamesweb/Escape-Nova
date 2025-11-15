using Code.Scripts.Core.Events;
using UnityEngine;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class StarsPanelUnlocker : MonoBehaviour
    {
        private void Awake()
        {
            SystemEvents.OnStarsPanelUnlocked += ShowPanel;

            if (SystemEvents.IsStarsPanelUnlocked)
            {
                ShowPanel();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            SystemEvents.OnStarsPanelUnlocked -= ShowPanel;
        }

        private void ShowPanel()
        {
            gameObject.SetActive(true);
        }
    }
}