using System;
namespace RailHexLib
{
    /// counts ticks and emit the Tick event
    public class Timer
    {
        public event Action OnTimeout;
        public int Ticks { get; private set; }
        public int Timeout { get; set; }
        public bool IsRunning { get; private set; }
        // for UI display timer
        public int RestTime => Timeout - Ticks;
        public void Tick(int ticks)
        {
            Ticks += ticks;
            if (Ticks >= Timeout)
            {
                Ticks %= Timeout;
                OnTimeout();
            }

        }
    };
}