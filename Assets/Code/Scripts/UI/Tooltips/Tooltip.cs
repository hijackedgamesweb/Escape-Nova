using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Code.Scripts.UI.Tooltips
{
    [ExecuteInEditMode]
    public class Tooltip : MonoBehaviour
    {
        [Header("Estilo Visual")]
        [SerializeField] private TMP_FontAsset tooltipFont;
        [SerializeField] private Vector2 cursorOffset = new Vector2(15f, 15f);
        
        [Header("Ajustes de Texto")]
        [SerializeField] private int characterWrapLimit = 80;

        [Header("Referencias")]
        [SerializeField] private TextMeshProUGUI headerField;
        [SerializeField] private TextMeshProUGUI contentField;
        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            ApplyStyle();
        }

        private void ApplyStyle()
        {
            if (tooltipFont != null)
            {
                if (headerField != null) headerField.font = tooltipFont;
                if (contentField != null) contentField.font = tooltipFont;
            }
            if (headerField != null) 
                headerField.alignment = TextAlignmentOptions.Center;
            
            if (contentField != null) 
                contentField.alignment = TextAlignmentOptions.Center;
        }

        public void SetText(string content, string header = "")
        {
            if (string.IsNullOrEmpty(header))
            {
                headerField.gameObject.SetActive(false);
            }
            else
            {
                headerField.gameObject.SetActive(true);
                headerField.text = header;
            }

            contentField.text = content;

            int headerLength = headerField.text.Length;
            int contentLength = contentField.text.Length;
            layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit);

            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            UpdatePosition();
        }

        private void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (Application.isPlaying && Mouse.current == null) return;
            
            Vector2 mousePos = Vector2.zero;
            if (Application.isPlaying)
            {
                 mousePos = Mouse.current.position.ReadValue();
            }
            else
            {
                return; 
            }

            float pivotX = mousePos.x / Screen.width;
            float pivotY = mousePos.y / Screen.height;
            
            rectTransform.pivot = new Vector2(pivotX, pivotY);

            float nudgeX = Mathf.Lerp(cursorOffset.x, -cursorOffset.x, pivotX);
            float nudgeY = Mathf.Lerp(cursorOffset.y, -cursorOffset.y, pivotY);

            transform.position = mousePos + new Vector2(nudgeX, nudgeY);
        }
        private void OnValidate()
        {
            ApplyStyle();
        }
    }
}