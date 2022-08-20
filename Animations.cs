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
            public readonly Sprite Sprite;
            public readonly float Duration;

            public Frame(Sprite sprite, float duration)
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

    public class Animator
    {
        private readonly List<Animation> animations;

        private int animationIndex = 0;
        private int frameIndex = 0;
        private float frameCounter = 0;

        public Animation? Animation
        {
            get
            {
                if (animationIndex >= 0 && animationIndex < animations.Count)
                    return animations[animationIndex];
                return null;
            }
        }

        private bool InValidState
        {
            get
            {
                return animationIndex >= 0
                       && animationIndex < animations.Count
                       && frameIndex >= 0
                       && frameIndex < animations[animationIndex].Frames.Count;
            }
        }

        public Animator(List<Animation> animations)
        {
            this.animations = animations;
        }

        public void Play(string animation)
        {
            for (int i = 0; i < animations.Count; i++)
            {
                if (animations[i].Name == animation)
                {
                    // Only start playing a given animation if it's different than
                    // the currently active animation.
                    if (animationIndex != i)
                    {
                        animationIndex = i;
                        frameIndex = 0;
                        frameCounter = 0;
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (InValidState)
            {
                var anim = animations[animationIndex];
                var frame = anim.Frames[frameIndex];

                frameCounter += gameTime.DeltaTime();

                while (frameCounter >= frame.Duration)
                {
                    frameCounter -= frame.Duration;
                    frameIndex++;

                    if (frameIndex >= anim.Frames.Count)
                        frameIndex = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (InValidState)
            {
                var anim = animations[animationIndex];
                var frame = anim.Frames[frameIndex];
                var sprite = frame.Sprite;
                sprite.Draw(spriteBatch);
            }
        }
    }
}
