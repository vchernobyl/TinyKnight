using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.GFX
{
    public class Sprite
    {
        public Vector2 Scale;
        public Vector2 Offset;
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

        public void Draw(Vector2 position)
        {
            if (Animation == null)
                return;

            var frame = Animation.CurrentFrame;
            var source = frame.Region;

            var spriteBatch = spriteSheet.SpriteBatch;
            spriteBatch.Begin(samplerState: SamplerState.PointClamp,
                transformMatrix: GravityGame.WorldCamera.Transform);
            {
                spriteBatch.Draw(spriteSheet.Texture, position + Offset, source, Color.White,
                    Rotation, Center, Scale, Flip, LayerDepth);
            }
            spriteBatch.End();
        }

        public void Draw(Vector2 position, Vector2 size)
        {
            if (Animation == null)
                return;

            var frame = Animation.CurrentFrame;
            var source = frame.Region;
            var destination = new Rectangle(
                (int)position.X, (int)position.Y,
                source.Width, source.Height);

            if (size.X > 0f)
                destination.Width = (int)size.X;
            if (size.Y > 0f)
                destination.Height = (int)size.Y;

            var spriteBatch = spriteSheet.SpriteBatch;
            spriteBatch.Begin(samplerState: SamplerState.PointClamp,
                transformMatrix: GravityGame.WorldCamera.Transform);
            {
                var texture = spriteSheet.Texture;
                spriteBatch.Draw(texture, destination, source, Color.White,
                    Rotation, Center, Flip, LayerDepth);
            }
            spriteBatch.End();
        }

        public void Play(int animationID, int startFrame = 0)
        {
            if (CurrentAnimationID == animationID)
                return;

            CurrentAnimationID = animationID;
            Animation = new AnimationTrack(spriteSheet.GetAnimation(animationID));
            Animation.Start(startFrame);
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

        public Vector2 Center
        {
            get { return FrameSize.ToVector2() / 2f; }
        }
    }
}
