using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public class Coin : Entity
    {
        private readonly SoundEffect pickupSound;

        public Coin(Game game)
            : base(game, new Sprite(game.Content.Load<Texture2D>("Textures/Coin")))
        {
            pickupSound = game.Content.Load<SoundEffect>("SoundFX/Coin_Pickup");
        }

        public override void OnEntityCollision(Entity other)
        {
            if (other is Hero hero)
            {
                hero.PickupCoin();
                pickupSound.Play();
                IsActive = false;
            }
        }
    }
}
