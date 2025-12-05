using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Scripts.UI.Tooltips
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [TextArea] public string header;
        [TextArea] public string content;

        public void OnPointerEnter(PointerEventData eventData)
        {
             TooltipSystem.Show(content, header);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipSystem.Hide();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TooltipSystem.Hide();
        }

        private void OnDisable()
        {
            TooltipSystem.Hide();
        }
    }
}