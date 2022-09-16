using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gravity.GFX
{
    public class SpriteSheet
    {
        private readonly List<Sprite> sprites;
        private readonly List<Animation> animations;
        private readonly Dictionary<string, int> animationIDMap;

        public SpriteSheet(GraphicsDevice device, Texture2D texture)
        {
            this.Texture = texture;
            this.SpriteBatch = new SpriteBatch(device);
            this.sprites = new List<Sprite>();
            this.animations = new List<Animation>();
            this.animationIDMap = new Dictionary<string, int>();
        }

        public Sprite Create()
        {
            var sprite = new Sprite(this);
            sprites.Add(sprite);
            return sprite;
        }

        public Animation CreateAnimation(string name, out int animationID)
        {
            if (GetAnimationID(name, out animationID))
                return animations[animationID];

            var anim = new Animation(name);
            animations.Add(anim);
            animationID = animations.Count - 1;
            animationIDMap[name] = animationID;

            return animations[animationID];
        }

        public bool GetAnimationID(string name, out int id)
        {
            id = int.MaxValue;

            if (!animationIDMap.ContainsKey(name))
                return false;

            id = animationIDMap[name];

            return true;
        }

        public Animation GetAnimation(int animationID)
        {
            Debug.Assert(animationID < animations.Count);
            return animations[animationID];
        }

        public SpriteBatch SpriteBatch { get; }

        public Texture2D Texture { get; }
    }
}
