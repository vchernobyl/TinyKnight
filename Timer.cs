using Microsoft.Xna.Framework;
using System;

namespace Gravity
{
    public class Timer
    {
        private readonly Action onEnd;
        private readonly double duration;
        private readonly bool repeating;

        private double time = 0;
        private bool started = false;

        public bool IsRunning => time > 0;

        public Timer(double duration, Action onEnd, bool repeating = false)
        {
            this.duration = duration;
            this.time = duration;
            this.onEnd = onEnd;
            this.repeating = repeating;
        }

        public void Start()
        {
            started = true;
        }

        public void Reset()
        {
            started = false;
            time = duration;
        }

        public void Update(GameTime gameTime)
        {
            if (started)
            {
                time -= gameTime.ElapsedGameTime.TotalSeconds;
                if (time <= 0)
                {
                    onEnd.Invoke();
                    if (repeating)
                        time = duration;
                }
            }
        }
    }
}
