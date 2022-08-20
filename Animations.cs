using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gravity
{
    public class AnimatedSprite
    {
        public class Frame
        {
            public readonly Subtexture Subtexture;
            public readonly float Duration;

            public Frame(Subtexture subtexture, float duration)
            {
                Subtexture = subtexture;
                Duration = duration;
            }
        }

        public class Animation
        {
            public readonly string Name;
            public readonly List<Frame> Frames;

            public float Duration => Frames.Sum(f => f.Duration);

            public Animation(string name, List<Frame> frames)
            {
                Name = name;
                Frames = frames;
            }
        }

        public readonly string Name;
        public readonly List<Animation> Animations;

        public Vector2 Position;

        public AnimatedSprite(string name, List<Animation> animations)
        {
            Name = name;
            Animations = animations;
        }

        public Animation GetAnimation(string name)
        {
            foreach (var a in Animations)
            {
                if (a.Name == name)
                    return a;
            }
            throw new System.ArgumentException($"Animation '{name}' could not be found.");
        }
    }

    public class Animator
    {
        private readonly AnimatedSprite animatedSprite;

        private int animationIndex = 0;
        private int frameIndex = 0;
        private float frameCounter = 0;

        public AnimatedSprite.Animation? Animation
        {
            get
            {
                if (animationIndex >= 0 && animationIndex < animatedSprite.Animations.Count)
                    return animatedSprite.Animations[animationIndex];
                return null;
            }
        }

        private bool InValidState
        {
            get
            {
                return animationIndex >= 0
                       && animationIndex < animatedSprite.Animations.Count
                       && frameIndex >= 0
                       && frameIndex < animatedSprite.Animations[animationIndex].Frames.Count;
            }
        }

        public Animator(AnimatedSprite animatedSprite)
        {
            this.animatedSprite = animatedSprite;
        }

        public void Play(string animation)
        {
            for (int i = 0; i < animatedSprite.Animations.Count; i++)
            {
                if (animatedSprite.Animations[i].Name == animation)
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
                var anim = animatedSprite.Animations[animationIndex];
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

        // TODO: Currently every Entity instance requires a Sprite.
        // However, some entities will have animated sprite instead.
        // We will remove sprite from the entity constructor and allow
        // of creation of entities without graphics (this is useful for
        // other purposes as well). Each entity subclass (hero, enemy, etc) 
        // will override Draw method and take care of rendering there -
        // either via sprite or animation drawing.
        public void Draw(SpriteBatch spriteBatch)
        {
            Debug.Assert(false);

            if (InValidState)
            {
                var anim = animatedSprite.Animations[animationIndex];
                var frame = anim.Frames[frameIndex];
                var subtexture = frame.Subtexture;
                spriteBatch.Draw(subtexture.Texture, animatedSprite.Position, subtexture.Source, Color.White);
            }
        }
    }
}
