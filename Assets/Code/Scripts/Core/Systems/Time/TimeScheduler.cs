using System;
using System.Collections.Generic;
using Code.Scripts.Core.Managers.Interfaces;
using Code.Scripts.Patterns.ServiceLocator;
using UnityEngine;

namespace Code.Scripts.Core.Systems.Time
{
    public class TimeScheduler : MonoBehaviour
    {
        private readonly List<Timer> _timers = new();
        private IGameTime _gameTime;
        
        private void Awake()
        {
            ServiceLocator.RegisterService<TimeScheduler>(this);
        }
        
        private void OnDestroy()
        {
            ServiceLocator.UnregisterService<TimeScheduler>();
        }

        private void Start()
        {
            _gameTime = ServiceLocator.GetService<IGameTime>();
        }

        private void Update()
        {
            if (_gameTime.IsPaused) return;

            float currentTime = _gameTime.GameTime;
            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                var timer = _timers[i];
                if (timer.Cancelled)
                {
                    _timers.RemoveAt(i);
                    continue;
                }

                if (currentTime >= timer.TriggerTime)
                {
                    try { 
                        timer.Callback?.Invoke(); 
                    }
                    catch (Exception ex) 
                    { 
                        Debug.LogError($"Timer callback exception: {ex}"); 
                    }
                    if (timer.Repeat)
                    {
                        timer.TriggerTime += timer.Interval;
                    }
                    else
                    {
                        _timers.RemoveAt(i);
                    }
                }
            }
        }
        
        public ITimerHandle CreateTimer(float delay, float interval, Action callback, bool repeat)
        {
            var timer = new Timer
            {
                TriggerTime = _gameTime.GameTime + delay,
                Interval = interval,
                Callback = callback,
                Repeat = repeat
            };
            _timers.Add(timer);
            return new TimerHandle(timer);
        }

        public ITimerHandle Schedule(float delay, Action callback) 
            => CreateTimer(delay, 0f, callback, false);
        
        public ITimerHandle ScheduleRepeating(float interval, Action callback)
            => CreateTimer(interval, interval, callback, true);
        
    }
}