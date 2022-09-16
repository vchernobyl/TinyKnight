using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.GFX
{
    public class Sprite
    {
        public Vector2 Offset;
        public Vector2 Origin;
        public Vector2 Scale;
        public float Rotation;
        public float LayerDepth;
        public SpriteEffects Flip;

        private readonly SpriteSheet spriteSheet;

        public Sprite(SpriteSheet spriteSheet)
        {
            this.spriteSheet = spriteSheet;
            this.CurrentAnimationID = int.MaxValue;
            Scale = Vector2.One;
        }

        public void Update(GameTime gameTime)
        {
            Animation?.Update(gameTime.DeltaTime());
        }

        public void Draw(Vector2 position, SpriteBatch spriteBatch)
        {
            if (Animation == null)
                return;

            var frame = Animation.CurrentFrame;
            var source = frame.Region;

            spriteBatch.Draw(spriteSheet.Texture, position + Offset, source, Color.White,
                Rotation, Origin, Scale, Flip, LayerDepth);
        }

        public void Play(int animationID, int startFrame = 0)
        {
            if (CurrentAnimationID == animationID)
                return;

            CurrentAnimationID = animationID;
            Animation = new AnimationTrack(spriteSheet.GetAnimation(animationID));
            Animation.Start(startFrame);
            Origin = FrameSize.ToVector2() / 2f;
        }

        public bool IsPlaying(int animationID)
        {
            return Animation != null && CurrentAnimationID == animationID;
        }

        public AnimationTrack Animation
        {
            get;
            private set;
        }

        public int CurrentAnimationID
        {
            get;
            private set;
        }

        public Point FrameSize
        {
            get { return Animation.CurrentFrame.Region.Size; }
        }
    }
}
