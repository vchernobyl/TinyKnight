namespace Gravity.Powerups
{
    public abstract class Powerup : Entity
    {
        public Powerup(GameplayScreen gameplayScreen) : base(gameplayScreen)
        {
            Category = Mask.Item;
            Collisions = Mask.Player | Mask.Level;
        }

        public abstract void ApplyEffect(Hero hero);

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Hero hero)
                ApplyEffect(hero);
        }
    }

    public interface IEffect
    {
        void ApplyEffect(Hero hero);
        void DiscardEffect(Hero hero);
    }
}
