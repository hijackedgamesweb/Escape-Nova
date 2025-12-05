using TMPro;
using UnityEngine;

namespace Code.Scripts.UI.Common
{
    public class FontController : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] textsToChange;

        public void SetFont(TMP_FontAsset font)
        {
            foreach (var text in textsToChange)
            {
                text.font = font;
            }
        }
    }
}