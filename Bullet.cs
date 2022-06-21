using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Bullet : Entity
    {
        // TODO: Feed the value via config file.
        public const int Damage = 50;

        public Vector2 Velocity { get; init; }

        private readonly float spreadVariation = 0.025f;
        private readonly float spread;

        public Bullet(Game game, Sprite sprite, Vector2 position, Vector2 velocity)
            : base(game, sprite)
        {
            SetCoordinates(position.X, position.Y);

            Velocity = velocity;

            spread = Random.FloatRange(-spreadVariation, spreadVariation);

            var sound = game.Content.Load<SoundEffect>("SoundFX/Pistol_Shot");
            sound.Play(volume: .7f, pitch: 0f, pan: 0f);
        }

        public override void Update(GameTime gameTime)
        {
            DX = Velocity.X;
            DY = spread;

            if (Velocity.X < 0)
                sprite.Flip = SpriteEffects.FlipHorizontally;
            else
                sprite.Flip = SpriteEffects.None;

            if (level.HasCollision(CX + 1, CY) ||
                level.HasCollision(CX - 1, CY))
                IsActive = false;

            base.Update(gameTime);
        }
    }
}
