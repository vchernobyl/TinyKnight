using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Gravity
{
    public class Animation
    {
        public class Frame
        {
            public readonly Subtexture Image;
            public readonly float Duration;

            public Frame(Subtexture image, float duration)
            {
                Image = image;
                Duration = duration;
            }
        }

        public readonly string Name;
        public readonly List<Frame> Frames;

        public float Duration => Frames.Sum(f => f.Duration);

        public Animation(string name, Texture2D animSheet, Point frameSize)
        {
            Name = name;
            
            Frames = new List<Frame>();

            var frameCount = animSheet.Width / frameSize.X;
            for (var i = 0; i < frameCount; i++)
            {
                var source = new Rectangle(i * frameSize.X, 0, frameSize.X, frameSize.Y);
                var subtexture = new Subtexture(animSheet, source);
                
                // Frame duration hardcoded for now.
                Frames.Add(new Frame(subtexture, duration: .1f));
            }
        }
    }
}
