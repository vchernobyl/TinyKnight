using Gravity.Graphics;
using Gravity.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity.Entities
{
    public class Chest : Entity
    {
        public Chest(GameplayScreen gameplayScreen) : base(gameplayScreen)
        {
            var content = gameplayScreen.ScreenManager.Game.Content;
            var spriteSheet = new SpriteSheet(content.Load<Texture2D>("Textures/Chest"));
            var anim = spriteSheet.CreateAnimation("Zombie_Walk", out int animID);
            anim.AddFrame(new Rectangle(0, 0, 8, 8), duration: 0f);

            sprite = spriteSheet.Create();
            sprite.LayerDepth = DrawLayer.Background;
            sprite.Play(animID);
        }

        public override void OnEntityCollisionEnter(Entity other)
        {
            if (other is Hero hero)
            {
                var weapon = new Cannon(hero, GameplayScreen);
                hero.EquipWeapon(weapon);
                var game = GameplayScreen.ScreenManager.Game;
                //`var text = new WeaponPickupText(game, weapon.Name, Position);
                //game.Components.Add(text);
            }
        }
    }
}
