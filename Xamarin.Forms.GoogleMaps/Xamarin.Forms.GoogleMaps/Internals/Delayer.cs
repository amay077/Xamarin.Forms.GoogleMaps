using System;
using Xamarin.Forms;

namespace Xamarin.Forms.Tools
{
    public class Delayer
    {
        public int DefaultNumberOfTicks { get; set; } = 10;
        /// <summary>Number of ticks before the action starts. -1=cancelled, 0=do, x = number of ticks missing</summary>
        public int Delay { get; private set; }
        /// <summary>Interval between ticks in milliseconds</summary>
        public int Interval { get; set; }

        public Delayer(int millisec)
        {
            Interval = millisec;
        }

        private bool TimerCallback()
        {
            if (Delay == -1) return false;
            if (--Delay > 0) { return true; }

            Action?.Invoke(this, new EventArgs());
            return false;
        }

        public event EventHandler Action;

        public void ResetAndTick()
        {
            if (Delay > 0)
                Delay = DefaultNumberOfTicks;
            else
            {
                Delay = DefaultNumberOfTicks;
                Device.StartTimer(TimeSpan.FromMilliseconds(Interval / DefaultNumberOfTicks), TimerCallback);
            }
        }

        public void Stop()
            => Delay = -1;
    }
}