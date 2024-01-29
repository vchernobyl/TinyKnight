using TinyKnight.Entities;
using TinyKnight.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TinyKnight.Weapons
{
    public class Flame : Entity
    {
        public Flame(GameplayScreen gameplayScreen) : base(gameplayScreen)
        {
            var content = GameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(8, 8 * 7, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Play(defaultAnimID);

            Category = Mask.PlayerProjectile;
            Collisions = Mask.Enemy | Mask.Level;

            FrictionX = 1f;
            FrictionY = 1f;
            Gravity = 0f;
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Enemy enemy)
            {
                enemy.Damage(25);
                Destroy();
            }
        }

        public override void OnLevelCollision(Vector2 normal)
        {
            Destroy();
        }

        public override void Update(GameTime gameTime)
        {
            Sprite.Rotation = Numerics.VectorToRadians(new Vector2(DX, DY));
        }
    }

    public class FireStaff : Weapon
    {
        public FireStaff(Hero hero) :
            base(hero, hero.GameplayScreen, fireRate: 5f, nameof(FireStaff), updateOrder: 200)
        {
            var content = hero.GameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Weapons"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 8 * 9, 8, 8), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.Play(defaultAnimID);
            Sprite.LayerDepth = DrawLayer.Foreground;

            Gravity = 0f;

            UpdatePosition();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdatePosition();
        }

        protected override void Shoot()
        {
            const float speed = .75f;

            var yVelocities = new float[] { 0f, -.1f, -.2f, -.3f };
            foreach (var yVel in yVelocities)
            {
                GameplayScreen.AddEntity(new Flame(GameplayScreen)
                {
                    Position = Position + new Vector2(6f * hero.Facing, -4f),
                    DX = hero.Facing * speed,
                    DY = yVel,
                });
            }
        }

        protected override void UpdatePosition()
        {
            Position = hero.Position + new Vector2(3f * hero.Facing, 1f);
            Sprite.Flip = hero.Facing > 0
                ? SpriteEffects.None
                : SpriteEffects.FlipHorizontally;
        }
    }
}
