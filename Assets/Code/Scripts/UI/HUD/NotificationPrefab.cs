using Code.Scripts.Core.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Code.Scripts.UI.HUD
{
    public class NotificationPrefab : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private float displayDuration = 3f;
        [SerializeField] private Image closeBtn;
        public void Initialize(string message, NotificationType type)
        {
            messageText.text = message;
            switch (type)
            {
                case NotificationType.Info:
                    iconImage.color = Color.green;
                    break;
                case NotificationType.Warning:
                    iconImage.color = Color.yellow;
                    break;
                case NotificationType.Error:
                    iconImage.color = Color.red;
                    break;
            }
            ShowNotification();
            closeBtn.GetComponent<Button>().onClick.AddListener(DestroyNotification);
        }

        public void ShowNotification()
        {
            // La notificacion entra desde fuera de la pantalla con un dotween o animacion
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(-rectTransform.rect.width, rectTransform.anchoredPosition.y);
            rectTransform.DOAnchorPosX(rectTransform.rect.width/2, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                Invoke(nameof(DestroyNotification), displayDuration);
            });
        }
        
        private void DestroyNotification()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.DOAnchorPosX(-rectTransform.rect.width, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
        
        
    }
}