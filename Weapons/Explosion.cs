using Gravity.Coroutines;
using Gravity.Entities;
using Gravity.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace Gravity.Weapons
{
    public class Explosion : Entity
    {
        public Explosion(GameplayScreen gameplayScreen)
            : base(gameplayScreen)
        {
            var game = gameplayScreen.ScreenManager.Game;
            var content = game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/ExplosionCircle"));
            var anim = spriteSheet.CreateAnimation("Default", out int defaultAnimID);
            anim.AddFrame(new Rectangle(0, 0, 32, 32), duration: 0f);

            Sprite = spriteSheet.Create();
            Sprite.Play(defaultAnimID);
            Sprite.LayerDepth = DrawLayer.Foreground;
            Sprite.Scale = Vector2.One * .25f;

            Gravity = 0f;

            var coroutine = game.Services.GetService<CoroutineRunner>();
            coroutine.Run(Expand());

            var sound = content.Load<SoundEffect>("SoundFX/Explosion_2");
            sound.Play();

            TinyKnightGame.WorldCamera.Shake(.55f);
        }

        public IEnumerator Expand()
        {
            var white = new Color(255, 241, 232);
            var yellow = new Color(255, 236, 39);

            var frames = 0;
            var totalDuration = 10f;
            while (frames++ < totalDuration)
            {
                if (frames <= 5)
                    Sprite.Color = yellow;
                else if (frames <= 10)
                    Sprite.Color = white;

                Sprite.Scale.X += .13f;
                Sprite.Scale.Y += .13f;

                const float spriteSize = 32f; // This needs to be changed if sprite size changes
                Radius = spriteSize / 2f * Sprite.Scale.X;

                yield return null;
            }
            Destroy();
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Enemy enemy)
            {
                enemy.Damage(999);
                var xDirection = Vector2.Normalize(enemy.Position - Position).X;
                enemy.DX = xDirection * .5f;
                enemy.DY = -.75f;
                enemy.Collisions = Mask.None;
            }
        }
    }
}
