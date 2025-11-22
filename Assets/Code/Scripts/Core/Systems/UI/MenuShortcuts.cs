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

            // LAS TECLAS VAN EN EL ORDEN DE QWERTY, siguiendo el criterio del orden del menu de juego.
            // R -> Storage
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                if (SystemEvents.IsInventoryUnlocked)
                {
                    OpenActionPanel("Storage");
                }
            }

            // E -> Research
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (SystemEvents.IsResearchUnlocked)
                {
                    OpenActionPanel("Research");
                }
            }

            // W -> Missions
            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                OpenActionPanel("Objectives");
            }

            // Y -> Constelaciones
            if (Keyboard.current.yKey.wasPressedThisFrame)
            {
                if (SystemEvents.IsConstellationsUnlocked)
                {
                    OpenActionPanel("Constellations");
                }
            }

            // Tt -> Astrario
            if (Keyboard.current.tKey.wasPressedThisFrame)
            {
                OpenActionPanel("Astrarium");
            }

            // Q -> Diplomacia
            if (Keyboard.current.qKey.wasPressedThisFrame)
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