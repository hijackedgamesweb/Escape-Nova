using UnityEngine;
using UnityEngine.EventSystems;
using Code.Scripts.Core.Systems.Resources;
using Code.Scripts.UI.HUD;

public class StorageIconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private StorageHUD storageHUD;
    [SerializeField] private ResourceType resourceType;

    public void OnPointerEnter(PointerEventData eventData)
    {
        storageHUD.ShowResourceName(resourceType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        storageHUD.HideAllNames();
    }
}
