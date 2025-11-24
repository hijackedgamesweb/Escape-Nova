using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace Code.Scripts.UI.Common
{
    public class FungusFontController : MonoBehaviour
    {
        [SerializeField] private Font[] fungusFontAssets;
        [SerializeField] private Text textComponent;
        [SerializeField] private Text nameTextComponent;
        public void SetMippipFont()
        {
            SetFontByIndex(1);
        }
        
        public void SetAkkiFont()
        {
            SetFontByIndex(5);
        }
        
        public void SetHandoullFont()
        {
            SetFontByIndex(2);
        }

        public void SetSkulgFont()
        {
            SetFontByIndex(3);
        }

        public void SetHalxiFont()
        {
            SetFontByIndex(4);
        }
        
        public void SetDefaultFont()
        {
            SetFontByIndex(0);
        }
        
        private void SetFontByIndex(int index)
        {
                if (index >= 0 && index < fungusFontAssets.Length)
                {
                    nameTextComponent.font = fungusFontAssets[index];
                    textComponent.font = fungusFontAssets[index];
                }
        }
    }
}
