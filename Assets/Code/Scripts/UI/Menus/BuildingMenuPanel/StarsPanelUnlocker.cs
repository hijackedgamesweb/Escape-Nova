using Code.Scripts.Core.Events;
using Code.Scripts.Core.SaveLoad.Interfaces;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Code.Scripts.UI.Menus.BuildingMenuPanel
{
    public class StarsPanelUnlocker : MonoBehaviour, ISaveable
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

        public string GetSaveId()
        {
            return "StarsPanelUnlocker";
        }


        public JToken CaptureState()
        {
            JObject state = new JObject
            {
                ["isUnlocked"] = SystemEvents.IsStarsPanelUnlocked,
                ["isVisible"] = gameObject.activeSelf
            };
            return state;
        }

        public void RestoreState(JToken state)
        {
            bool isUnlocked = state.Value<bool>("isUnlocked");
            bool isVisible = state.Value<bool>("isVisible");

            if(isUnlocked)
            {
                SystemEvents.UnlockStarsPanel();
            }
            gameObject.SetActive(isVisible && isUnlocked);
        }
    }
}