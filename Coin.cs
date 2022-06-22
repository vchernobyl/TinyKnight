namespace Gravity
{
    public class Coin : Entity
    {
        public Coin(Game game) : base(game, new Sprite(Textures.Coin))
        {
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Hero hero)
            {
                hero.PickupCoin();
                SoundFX.CoinPickup.Play();
                IsActive = false;
            }
        }
    }
}
