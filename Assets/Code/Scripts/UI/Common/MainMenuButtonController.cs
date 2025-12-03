using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Scripts.UI.Common
{
    public class MainMenuButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        }
    }
}
