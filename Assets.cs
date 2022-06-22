using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Gravity
{
    public static class Assets
    {
        public static void Load(ContentManager content)
        {
            Textures.Load(content);
            SoundFX.Load(content);
            Fonts.Load(content);
            Effects.Load(content);
        }
    }

    public static class Textures
    {
        public static Texture2D Bullet { get; private set; }
        public static Texture2D MuzzleFlash { get; private set; }
        public static Texture2D Coin { get; private set; }
        public static Texture2D Hero { get; private set; }

        public static void Load(ContentManager content)
        {
            Bullet = content.Load<Texture2D>("Textures/bullet");
            MuzzleFlash = content.Load<Texture2D>("Textures/Muzzle_Flash");
            Coin = content.Load<Texture2D>("Textures/Coin");
            Hero = content.Load<Texture2D>("Textures/character_0000");
        }
    }

    public static class SoundFX
    {
        public static SoundEffect PistolShot { get; private set; }
        public static SoundEffect HeroJump { get; private set; }
        public static SoundEffect EnemyHit { get; private set; }
        public static SoundEffect CoinPickup { get; private set; }

        public static void Load(ContentManager content)
        {
            PistolShot = content.Load<SoundEffect>("SoundFX/Pistol_Shot");
            HeroJump = content.Load<SoundEffect>("SoundFX/Hero_Jump");
            EnemyHit = content.Load<SoundEffect>("SoundFX/Enemy_Hit");
            CoinPickup = content.Load<SoundEffect>("SoundFX/Coin_Pickup");
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
