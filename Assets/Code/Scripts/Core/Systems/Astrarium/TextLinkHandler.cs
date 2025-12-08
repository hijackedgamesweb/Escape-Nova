using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextLinkHandler : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI _text;
    [SerializeField] private Code.Scripts.UI.Menus.AstrariumUIController _controller;

    private void Awake() => _text = GetComponent<TextMeshProUGUI>();

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, Input.mousePosition, null);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = _text.textInfo.linkInfo[linkIndex];
            string linkID = linkInfo.GetLinkID();
        }
    }
}