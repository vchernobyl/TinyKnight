using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Bullet : Entity
    {
        // TODO: Feed the value via config file.
        public const int Damage = 50;

        public int Direction { get; init; }
        public float Speed { get; init; }

        public Bullet(Game game, Sprite sprite, Level level, Vector2 position)
            : base(game, sprite, level)
        {
            SetCoordinates(position.X, position.Y);

            sprite.Scale = new Vector2(1.5f, 1.5f);

            var sound = game.Content.Load<SoundEffect>("SoundFX/Shot");
            sound.Play(volume: .7f, pitch: 0f, pan: 0f);
        }

        public override void Update(GameTime gameTime)
        {
            DX = Direction * Speed;
            DY = 0f;

            if (Direction < 0)
                sprite.Effect = SpriteEffects.FlipHorizontally;
            else
                sprite.Effect = SpriteEffects.None;

            if (level.HasCollision(CX + 1, CY) ||
                level.HasCollision(CX - 1, CY))
                IsActive = false;

            base.Update(gameTime);
        }
    }
}
