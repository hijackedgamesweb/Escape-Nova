using Code.Scripts.Patterns.Singleton;
using Code.Scripts.UI.HUD;
using UnityEngine;

namespace Code.Scripts.Core.Managers
{
    public class NotificationManager : Singleton<NotificationManager>
    { 
        [SerializeField] private NotificationPrefab notificationPrefab;
    
        public void CreateNotification(string message, NotificationType type)
        {
            NotificationPrefab notification = Instantiate(notificationPrefab, this.transform);
            notification.Initialize(message, type);
        }
    }
    
    public enum NotificationType
    {
        Info,
        Warning,
        Error
    }
}
