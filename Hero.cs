﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class Hero : Entity
    {
        private readonly SoundEffect jumpSound;
        private readonly Sprite muzzleSprite;

        private bool onGround = false;
        private int facing = -1;

        private double shotTimer = .0;
        private double muzzleTimer = .0;

        public Hero(Game game, Sprite sprite, Level level)
            : base(game, sprite, level)
        {
            jumpSound = game.Content.Load<SoundEffect>("SoundFX/Hero_Jump");
            muzzleSprite = new Sprite(game.Content.Load<Texture2D>("Textures/Muzzle_Flash"))
            {
                LayerDepth = .1f
            };
        }

        public override void Update(GameTime gameTime)
        {
            var speed = .15f;
            var jump = -1f;

            if (Keyboard.IsKeyDown(Keys.Left))
            {
                sprite.Effect = SpriteEffects.None;
                DX = -speed;
                facing = -1;
            }
            if (Keyboard.IsKeyDown(Keys.Right))
            {
                sprite.Effect = SpriteEffects.FlipHorizontally;
                DX = speed;
                facing = 1;
            }

            if (Keyboard.WasKeyPressed(Keys.Up) && onGround)
            {
                DY = jump;
                jumpSound.Play(.7f, 0f, 0f);
            }

            shotTimer += gameTime.ElapsedGameTime.TotalSeconds;
            muzzleTimer -= gameTime.ElapsedGameTime.TotalSeconds;

            if (Keyboard.IsKeyDown(Keys.Space) && shotTimer >= .15)
            {
                shotTimer = 0.0;
                muzzleTimer = .0125;

                var sprite = new Sprite(game.Content.Load<Texture2D>("Textures/bullet"));
                var position = Position + Vector2.UnitX * facing * Level.CellSize;
                var bullet = new Bullet(game, sprite, level, position)
                {
                    Direction = facing,
                    Speed = 1f,
                };
                game.AddEntity(bullet);
            }

            onGround = level.HasCollision(CX, CY + 1);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);

            if (muzzleTimer >= .0)
            {
                var position = Position + Vector2.UnitX * facing * Level.CellSize;
                muzzleSprite.Position = position;
                muzzleSprite.Draw(batch);
            }
        }
    }
}
