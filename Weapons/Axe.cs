using Gravity.Coroutines;
using Gravity.Entities;
using Gravity.GFX;
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
        private float throwTime;
        private int direction;

        public Axe(Hero hero, GameplayScreen gameplayScreen)
            : base(hero, gameplayScreen, fireRate: 1f, nameof(Axe), updateOrder: 100)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 8, 8, 8), 0f);

            sprite = spriteSheet.Create();
            sprite.Play(defaultAnimID);
            sprite.LayerDepth = 1f;

            axeState = FlyStage.InHands;

            Gravity = 0f;
            LevelCollisions = false;
            EntityCollisions = true;
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if ((axeState == FlyStage.Flying || axeState == FlyStage.Returning) && other is Enemy enemy)
                enemy.Damage(10);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            const float rotationSpeed = 0f;
            const float flyingSpeed = .75f;
            const float maxFlyTime = .5f;

            switch (axeState)
            {
                case FlyStage.InHands:
                    Position = hero.Position;
                    break;
                case FlyStage.Flying:
                    var force = direction * flyingSpeed;
                    DX = force;
                    throwTime += gameTime.DeltaTime();

                    sprite.Rotation += rotationSpeed;

                    if (throwTime >= maxFlyTime)
                    {
                        throwTime = 0f;
                        axeState = FlyStage.Returning;
                    }
                    break;
                case FlyStage.Returning:
                    var dir = Vector2.Normalize(hero.Position - Position);
                    (DX, DY) = dir * flyingSpeed;

                    sprite.Rotation += rotationSpeed;

                    if (Vector2.Distance(hero.Position, Position) <= 5f)
                    {
                        axeState = FlyStage.InHands;
                        Position = hero.Position;

                        DX = 0f;
                        DY = 0f;

                        sprite.Rotation = 0f;
                    }

                    break;
            }
        }

        public override void Shoot()
        {
            if (axeState == FlyStage.InHands)
            {
                axeState = FlyStage.Flying;
                direction = hero.Facing;
            }
        }
    }
}
