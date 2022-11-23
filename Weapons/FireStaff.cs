using Gravity.Coroutines;
using Gravity.Graphics;
using Gravity.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace Gravity.Weapons
{
    public class Flame : Entity
    {
        private readonly Game game;
        private readonly ParticleSystem fireParticles;
        private readonly ParticleEmitter fireEmitter;

        public Flame(GameplayScreen gameplayScreen) : base(gameplayScreen)
        {
            game = gameplayScreen.ScreenManager.Game;
            fireParticles = new ParticleSystem(game, "Particles/Fire");
            game.Components.Add(fireParticles);
            fireEmitter = new ParticleEmitter(fireParticles, 30, Position);

            var coroutine = game.Services.GetService<CoroutineRunner>();
            coroutine.Run(DestroyAfter(delay: 2f));
        }

        public override void Update(GameTime gameTime)
        {
            fireEmitter.Update(gameTime, Position);
        }

        private IEnumerator DestroyAfter(float delay)
        {
            yield return delay;
            Destroy();
            yield return delay;
            game.Components.Remove(fireParticles);
        }
    }

    public class FireStaff : Weapon
    {
        public FireStaff(Hero hero) :
            base(hero, hero.GameplayScreen, 12f, nameof(FireStaff), updateOrder: 200)
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
            var flame = new Flame(GameplayScreen)
            {
                Position = Position,
                DX = hero.Facing
            };
            GameplayScreen.AddEntity(flame);
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
