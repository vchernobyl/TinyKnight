using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.GFX
{
    public class Sprite
    {
        private readonly SpriteSheet spriteSheet;

        private AnimationTrack currentAnimation;
        private int currentAnimationID;

        public Sprite(SpriteSheet spriteSheet)
        {
            this.spriteSheet = spriteSheet;
            this.currentAnimationID = int.MaxValue;
        }

        public void Update(float deltaTime)
        {
            currentAnimation?.Update(deltaTime);
        }

        public void Draw(
            Vector2 position, Color color,
            float rotation, Vector2 origin, SpriteEffects effect)
        {
            Draw(position, color, rotation, origin,
                Rectangle.Empty, Vector2.Zero, effect);
        }

        public void Draw(
            Vector2 position, Color color, float rotation,
            Vector2 origin, Rectangle clipping, Vector2 size,
            SpriteEffects effect, float depth = 0f)
        {
            if (currentAnimation == null)
                return;

            var frame = currentAnimation.CurrentFrame;
            var source = frame.Region;
            var destination = new Rectangle(
                (int)position.X, (int)position.Y,
                source.Width, source.Height);

            if (size.X > 0f)
                destination.Width = (int)size.X;
            if (size.Y > 0f)
                destination.Height = (int)size.Y;

            var spriteBatch = spriteSheet.SpriteBatch;
            spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                transformMatrix: GravityGame.WorldCamera.Transform);
            {
                var texture = spriteSheet.Texture;
                spriteBatch.Draw(texture, destination, source, color,
                    rotation, origin, effect, depth);
            }
            spriteBatch.End();
        }

        public void Play(int animationID, int startFrame = 0)
        {
            if (currentAnimationID == animationID)
                return;

            currentAnimationID = animationID;
            currentAnimation = new AnimationTrack(spriteSheet.GetAnimation(animationID));
            currentAnimation.Start(startFrame);
        }

        public bool IsPlaying(int animationID)
        {
            return currentAnimation != null && currentAnimationID == animationID;
        }

        public AnimationTrack Animation
        {
            get { return currentAnimation; }
        }

        public int CurrentAnimationID
        {
            get { return currentAnimationID; }
        }

        public Point FrameSize
        {
            get { return currentAnimation.CurrentFrame.Region.Size; }
        }
    }
}
