using Code.Scripts.Core.Systems.Time;

namespace Code.Scripts.Core.Managers.Interfaces
{
    public class TimerHandle : ITimerHandle
    {
        private Timer _timer;
        public TimerHandle(Timer timer)
        {
            _timer = timer;
        }
        public void Cancel()
        {
            _timer.Cancelled = true;
        }
    }
}