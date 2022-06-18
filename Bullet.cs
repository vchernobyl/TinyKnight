using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    public class Bullet : Entity
    {
        // TODO: Feed the value via config file.
        public const int Damage = 50;

        public int Direction { get; init; }
        public float Speed { get; init; }

        private readonly float spreadVariation = 0.05f;
        private readonly float spread;

        public Bullet(Game game, Sprite sprite, Level level, Vector2 position)
            : base(game, sprite, level)
        {
            SetCoordinates(position.X, position.Y);

            sprite.Scale = new Vector2(1.5f, 1.5f);
            spread = RNG.FloatRange(-spreadVariation, spreadVariation);

            var sound = game.Content.Load<SoundEffect>("SoundFX/Pistol_Shot");
            sound.Play(volume: .7f, pitch: 0f, pan: 0f);
        }

        public override void Update(GameTime gameTime)
        {
            DX = Direction * Speed;
            DY = spread;

            if (Direction < 0)
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
