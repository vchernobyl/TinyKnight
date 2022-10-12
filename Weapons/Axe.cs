using Gravity.Entities;
using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Weapons
{
    public class Axe : Weapon
    {
        private enum FlyStage
        {
            InHands,
            Flying,
            Returning,
        }

        private FlyStage axeState;
        private int direction;
        private float speed;

        public Axe(Hero hero, GameplayScreen gameplayScreen)
            : base(hero, gameplayScreen, fireRate: 3f, nameof(Axe), updateOrder: 100)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 8, 8, 8), 0f);

            sprite = spriteSheet.Create();
            sprite.Play(defaultAnimID);
            sprite.LayerDepth = DrawLayer.Foreground;

            axeState = FlyStage.InHands;

            Gravity = 0f;
            LevelCollisions = false;
            EntityCollisions = true;
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if ((axeState == FlyStage.Flying || axeState == FlyStage.Returning) && other is Enemy enemy)
                enemy.Damage(50);

            if (axeState == FlyStage.Returning && other is Hero)
            {
                axeState = FlyStage.InHands;
                Position = hero.Position;

                DX = 0f;
                DY = 0f;

                sprite.Rotation = 0f;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            const float rotationSpeed = .35f;
            const float acceleration = .085f;
            const float deceleration = .085f;

            var rotation = rotationSpeed * direction;

            switch (axeState)
            {
                case FlyStage.InHands:
                    Position = hero.Position;
                    break;
                case FlyStage.Flying:
                    speed -= deceleration;
                    DX = direction * speed;

                    sprite.Rotation += rotation;

                    if ((direction > 0 && DX < 0f) || (direction < 0 && DX > 0))
                    {
                        axeState = FlyStage.Returning;
                        speed = 0f;
                        DX = 0f;
                    }
                    break;
                case FlyStage.Returning:
                    var dir = Vector2.Normalize(hero.Position - Position);
                    speed += acceleration;
                    DX = dir.X * speed;
                    DY = dir.Y * speed;

                    sprite.Rotation += rotation;

                    break;
            }
        }

        public override void Shoot()
        {
            if (axeState == FlyStage.InHands)
            {
                axeState = FlyStage.Flying;
                direction = hero.Facing;
                speed = 1.75f;
            }
        }
    }
}
