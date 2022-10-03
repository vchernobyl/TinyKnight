using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Gravity
{
    [Obsolete("Load content directly from ContentManager")]
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

    [Obsolete("Load content directly from ContentManager")]
    public static class Effects
    {
        public static Effect Flash { get; private set; }

        public static void Load(ContentManager content)
        {
            Flash = content.Load<Effect>("Effects/FlashEffect");
        }
    }
}
