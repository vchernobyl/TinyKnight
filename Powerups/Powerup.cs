using Microsoft.Xna.Framework;

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
            if (other is Hero hero)
            {
                hero.ApplyEffect(CreateEffect());
                Destroy();
            }
        }

        protected abstract PowerupEffect CreateEffect();
    }

    public abstract class PowerupEffect
    {
        protected readonly Hero hero;
        protected readonly float duration;

        private float time;

        public PowerupEffect(Hero hero, float duration)
        {
            this.hero = hero;
            this.duration = duration;
        }

        public abstract void On();
        public abstract void Off();

        public void Update(GameTime gameTime)
        {
            OnUpdate(gameTime);

            time += gameTime.DeltaTime();
            if (time >= duration)
                hero.DiscardEffect();
        }

        protected virtual void OnUpdate(GameTime gameTime) { }
    }
}
