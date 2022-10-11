using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Gravity.Graphics
{
    public class Animation
    {
        public class Frame
        {
            public readonly Rectangle Region;
            public readonly float Duration;

            public Frame(Rectangle region, float duration)
            {
                Region = region;
                Duration = duration;
            }
        }

        public readonly string Name;
        public bool Looping = true;

        private readonly List<Frame> frames;

        public Animation(string name)
        {
            this.Name = name;
            this.frames = new List<Frame>();
        }

        public Frame GetFrame(int index)
        {
            return frames[index];
        }

        public Frame AddFrame(Rectangle region, float duration)
        {
            var frame = new Frame(region, duration);
            frames.Add(frame);
            return frame;
        }

        public int FrameCount
        {
            get { return frames.Count; }
        }
    }

}
