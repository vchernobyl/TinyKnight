using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gravity
{
    public class Animator
    {
        public Vector2 Position;

        private readonly List<Animation> animations;

        private int animationIndex = 0;
        private int frameIndex = 0;
        private float frameCounter = 0;

        public Animation Animation
        {
            get
            {
                Debug.Assert(animationIndex >= 0 && animationIndex < animations.Count);
                return animations[animationIndex];
            }
        }

        public Animation.Frame Frame
        {
            get
            {
                Debug.Assert(animationIndex >= 0 && animationIndex < animations.Count);
                var anim = animations[animationIndex];

                Debug.Assert(frameIndex >= 0 && frameIndex < anim.Frames.Count);
                var frame = anim.Frames[frameIndex];

                return frame;
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
                    break;
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

                    // Later on if we decide that we also want non-looping animations,
                    // we can add an additional flag to check whether the animation should
                    // be reset or not.
                    if (frameIndex >= anim.Frames.Count)
                        frameIndex = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (InValidState)
            {
                var sprite = Frame.Sprite;
                sprite.Position = Position;
                sprite.Draw(spriteBatch);
            }
        }
    }
}
