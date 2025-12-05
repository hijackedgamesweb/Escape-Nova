using Code.Scripts.UI.Common;
using TMPro;
using UnityEngine;

namespace Code.Scripts.UI.Menus.MainMenu
{
    public class MainMenuPlanetButton : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset font;
        [SerializeField] FontController _fontController;
        
        public void OnMouseDown()
        {
            _fontController.SetFont(font);
        }
    }
}
