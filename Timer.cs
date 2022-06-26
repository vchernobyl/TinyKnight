using Microsoft.Xna.Framework;
using System;

namespace Gravity
{
    public class Timer
    {
        private readonly Action onEnd;

        private double duration;
        private bool started = false;

        public Timer(double duration, Action onEnd)
        {
            this.duration = duration;
            this.onEnd = onEnd;
        }

        public void Start()
        {
            started = true;
        }

        public void Update(GameTime gameTime)
        {
            if (started)
            {
                duration -= gameTime.ElapsedGameTime.TotalSeconds;
                if (duration <= 0)
                    onEnd.Invoke();
            }
        }
    }
}
