using UnityEngine;
using UnityEngine.InputSystem;
using Code.Scripts.Core.Managers;
using Code.Scripts.UI.Windows;
using Code.Scripts.Core.Events;

namespace Code.Scripts.UI.Common
{
    public class MenuShortcuts : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current == null) return;

            // S -> Storage
            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                if (SystemEvents.IsInventoryUnlocked)
                {
                    OpenActionPanel("Storage");
                }
            }

            // R -> Research
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                if (SystemEvents.IsResearchUnlocked)
                {
                    OpenActionPanel("Research");
                }
            }

            // M -> Missions
            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                OpenActionPanel("Objectives");
            }

            // C -> Constelaciones
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                if (SystemEvents.IsConstellationsUnlocked)
                {
                    OpenActionPanel("Constellations");
                }
            }

            // A -> Astrario
            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                OpenActionPanel("Astrarium");
            }

            // D -> Diplomacia
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                OpenActionPanel("Diplomacy");
            }

            // P -> Volver / Pausa
            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                UIManager.Instance.ShowScreen<InGameScreen>();
            }
        }

        private void OpenActionPanel(string panelId)
        {
            UIManager.Instance.ShowScreen<ActionPanelScreen>(panelId);
        }
    }
}