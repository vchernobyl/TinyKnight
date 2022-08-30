using System.Collections.Generic;
using System.Linq;

namespace Gravity
{
    public class Animation
    {
        public class Frame
        {
            public readonly Sprite Sprite;
            public readonly float Duration;

            public Frame(Sprite sprite, float duration = .1f)
            {
                Sprite = sprite;
                Duration = duration;
            }
        }

        public readonly string Name;
        public readonly List<Frame> Frames;

        public float Duration => Frames.Sum(f => f.Duration);

        public Animation(string name, List<Frame> frames)
        {
            Name = name;
            Frames = frames;
        }
    }
}
