using UnityEngine;
using Code.Scripts.Core.Managers;
using Code.Scripts.UI.Menus;
using Code.Scripts.Camera;
using UnityEngine.EventSystems;

namespace Code.Scripts.Core.World
{
    public class SunInteractable : MonoBehaviour
    {
        private void OnMouseDown()
        {
            // Evitar clic a través de la UI
            if (EventSystem.current.IsPointerOverGameObject()) return;

            AudioManager.Instance.PlaySFX("ButtonClick"); // O un sonido de sol
            
            // Enfocar cámara en el sol
            if (UnityEngine.Camera.main != null)
            {
                var cam = UnityEngine.Camera.main.GetComponent<CameraController2D>();
                if (cam != null) cam.SetTarget(this.transform);
            }

            // Abrir el panel de información global
            SunInfoPanel.Instance.ShowPanel();
        }
    }
}