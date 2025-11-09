using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableModal : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform dragRectTransform;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        if (dragRectTransform == null)
            dragRectTransform = GetComponent<RectTransform>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Llevar al frente cuando se hace clic
        if (dragRectTransform != null)
            dragRectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragRectTransform == null || canvas == null) return;

        // Mover el modal según el movimiento del ratón
        dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}