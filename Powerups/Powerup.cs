using Gravity.Coroutines;
using System.Collections;

namespace Gravity.Powerups
{
    public abstract class Powerup : Entity
    {
        public Powerup(GameplayScreen gameplayScreen) : base(gameplayScreen)
        {
            Category = Mask.Item;
            Collisions = Mask.Player | Mask.Level;
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Hero)
            {
                var game = GameplayScreen.ScreenManager.Game;
                var coroutine = game.Services.GetService<CoroutineRunner>();
                var effect = CreateEffect();
                coroutine.Run(effect.ApplyEffect());
                Destroy();
            }
        }

        protected abstract Effect CreateEffect();
    }

    public abstract class Effect
    {
        public readonly Hero Hero;
        public readonly float Duration;

        public Effect(Hero hero, float duration)
        {
            Hero = hero;
            Duration = duration;
        }

        protected abstract void EffectOn();
        protected abstract void EffectOff();

        public IEnumerator ApplyEffect()
        {
            EffectOn();
            yield return Duration;
            EffectOff();
        }
    }
}
