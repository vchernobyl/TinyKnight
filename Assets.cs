using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public static class Textures
    {
        public static Texture2D Bullet { get; private set; }
        public static Texture2D Pellet { get; private set; }
        public static Texture2D MuzzleFlash { get; private set; }
        public static Texture2D Coin { get; private set; }
        public static Texture2D Hero { get; private set; }
        public static Texture2D Enemy { get; private set; }
        public static Texture2D Flyer { get; private set; }
        public static Texture2D PortalOrange { get; private set; }
        public static Texture2D PortalYellow { get; private set; }
        public static Texture2D Pixel { get; private set; }
        public static Texture2D Circle { get; private set; }
        public static Texture2D Box { get; private set; }

        public static void Load(ContentManager content)
        {
            Bullet = content.Load<Texture2D>("Textures/bullet");
            Pellet = content.Load<Texture2D>("Textures/Pellet");
            MuzzleFlash = content.Load<Texture2D>("Textures/Muzzle_Flash");
            Coin = content.Load<Texture2D>("Textures/Coin");
            Hero = content.Load<Texture2D>("Textures/character_0000");
            Enemy = content.Load<Texture2D>("Textures/character_0015");
            Flyer = content.Load<Texture2D>("Textures/Flyer");
            PortalOrange = content.Load<Texture2D>("Textures/Portal_Orange");
            PortalYellow = content.Load<Texture2D>("Textures/Portal_Yellow");
            Pixel = content.Load<Texture2D>("Textures/Pixel");
            Circle = content.Load<Texture2D>("Textures/Circle");
            Box = content.Load<Texture2D>("Textures/Box");
        }
    }

    public static class SoundFX
    {
        public static SoundEffect PistolShot { get; private set; }
        public static SoundEffect ShotgunShot { get; private set; }
        public static SoundEffect HeroJump { get; private set; }
        public static SoundEffect EnemyHit { get; private set; }
        public static SoundEffect CoinPickup { get; private set; }
        public static SoundEffect HeroHurt { get; private set; }
        public static SoundEffect BazookaShot { get; private set; }
        public static SoundEffect Explosion { get; private set; }
        public static SoundEffect PortalExplosionWindup { get; private set; }
        public static SoundEffect PortalExplosion { get; private set; }


        public static void Load(ContentManager content)
        {
            PistolShot = content.Load<SoundEffect>("SoundFX/Pistol_Shot");
            ShotgunShot = content.Load<SoundEffect>("SoundFX/Shotgun_Shot");
            HeroJump = content.Load<SoundEffect>("SoundFX/Hero_Jump");
            EnemyHit = content.Load<SoundEffect>("SoundFX/Enemy_Hit");
            CoinPickup = content.Load<SoundEffect>("SoundFX/Coin_Pickup");
            HeroHurt = content.Load<SoundEffect>("SoundFX/Hero_Hurt");
            BazookaShot = content.Load<SoundEffect>("SoundFX/Bazooka_Shot");
            Explosion = content.Load<SoundEffect>("SoundFX/Explosion");
            PortalExplosionWindup = content.Load<SoundEffect>("SoundFX/Portal_Explosion_Windup");
            PortalExplosion = content.Load<SoundEffect>("SoundFX/Portal_Explosion");
        }
    }

    public static class Fonts
    {
        public static SpriteFont Default { get; private set; }

        public static void Load(ContentManager content)
        {
            Default = content.Load<SpriteFont>("Fonts/Default");
        }
    }

    public static class Effects
    {
        public static Effect Flash { get; private set; }

        public static void Load(ContentManager content)
        {
            Flash = content.Load<Effect>("Effects/FlashEffect");
        }
    }
}
