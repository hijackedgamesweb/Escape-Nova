using UnityEngine;
using Code.Scripts.Patterns.ServiceLocator;
using Code.Scripts.Core.Managers;
using Code.Scripts.Core.Systems.Time;

public class ServiceInitializer : MonoBehaviour
{
    [Header("Services to Initialize")]
    [SerializeField] private bool initializeGameTime = true;
    [SerializeField] private bool initializeTimeScheduler = true;

    private void Awake()
    {
        Debug.Log("ServiceInitializer: Starting service initialization...");

        if (initializeGameTime)
        {
            var gameTime = FindObjectOfType<GameTimeManager>();
            if (gameTime != null)
            {
                Debug.Log("ServiceInitializer: GameTimeManager found and registered");
            }
            else
            {
                Debug.LogError("ServiceInitializer: GameTimeManager not found in scene!");
            }
        }

        if (initializeTimeScheduler)
        {
            var timeScheduler = FindObjectOfType<TimeScheduler>();
            if (timeScheduler != null)
            {
                Debug.Log("ServiceInitializer: TimeScheduler found and registered");
            }
            else
            {
                Debug.LogError("ServiceInitializer: TimeScheduler not found in scene!");
            }
        }

        Debug.Log("ServiceInitializer: Service initialization completed");
    }
}