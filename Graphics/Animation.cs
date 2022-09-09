using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Gravity.Graphics
{
    public class Animation
    {
        // For this game we will just hardcode the frame size for
        // convenience, since all of the sprites will be of the same size.
        private const int FrameSize = 8;

        public readonly string Name;
        public readonly List<Frame> Frames;

        public float Duration => Frames.Sum(f => f.Duration);

        public Animation(string name, Texture2D animSheet)
        {
            Name = name;

            Frames = new List<Frame>();

            var frameCount = animSheet.Width / FrameSize;
            for (var i = 0; i < frameCount; i++)
            {
                var source = new Rectangle(i * FrameSize, 0, FrameSize, FrameSize);
                var subtexture = new Subtexture(animSheet, source);

                // Frame duration hardcoded for now.
                Frames.Add(new Frame(subtexture, duration: .1f));
            }
        }
    }
}
