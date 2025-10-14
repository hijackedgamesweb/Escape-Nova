using System;

namespace Code.Scripts.Core.Systems.Time
{
    public class Timer
    {
        public float TriggerTime;
        public float Interval;
        public Action Callback;
        public bool Repeat;
        public bool Cancelled;
    }
}