using System;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Core.Systems.Time;
using Code.Scripts.Patterns.ServiceLocator;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Test
{
    public class TimerScheduleTaskTest : MonoBehaviour
    {
        private TimeScheduler _scheduler;
        private ITimerHandle _timer;

        public float UniqueTaskTimer = 5f;
        public float IntervalTaskTimer = 2f;
        
        [SerializeField] Button _uniqueTaskButton;
        [SerializeField] Button _intervalTaskButton;
        
        private void Start()
        {
            _scheduler = ServiceLocator.GetService<TimeScheduler>();
        }

        public void UniqueTask()
        {
            _scheduler.Schedule(UniqueTaskTimer, ShakeUniqueButton);
        }
    
        public void IntervalTask()
        {
            _scheduler.ScheduleRepeating(IntervalTaskTimer, ShakeIntervalButton);
        }
        
        public void ShakeUniqueButton()
        {
            _uniqueTaskButton.transform.DOShakePosition(1f, strength: new Vector3(10f, 0f, 0f), vibrato: 10, randomness: 90, snapping: false, fadeOut: true);
        }
        
        public void ShakeIntervalButton()
        {
            _intervalTaskButton.transform.DOShakePosition(1f, strength: new Vector3(10f, 0f, 0f), vibrato: 10, randomness: 90, snapping: false, fadeOut: true);
        }
    }
}
