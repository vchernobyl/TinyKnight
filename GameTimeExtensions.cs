using Microsoft.Xna.Framework;

namespace Gravity
{
    public static class GameTimeExtensions
    {
        public static double DeltaTime(this GameTime gameTime)
        {
            return gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
