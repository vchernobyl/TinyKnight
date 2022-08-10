using Microsoft.Xna.Framework;
using System;

namespace Gravity
{
    public class Timer
    {
        public bool Started { get; private set; } = false;

        private readonly Action onEnd;
        private readonly double duration;
        private readonly bool repeating;

        private double time = 0;

        public Timer(float duration, Action onEnd,
            bool repeating = false, bool immediate = false)
        {
            this.duration = duration;
            this.time = immediate ? 0 : duration;
            this.onEnd = onEnd;
            this.repeating = repeating;
        }

        public void Start()
        {
            Started = true;
        }

        public void Reset()
        {
            Started = false;
            time = duration;
        }

        public void Update(GameTime gameTime)
        {
            if (Started)
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
