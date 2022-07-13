using System;

namespace Gravity
{
    /// <summary>
    /// Our game's implementation of IScreenFactory which can handle
    /// creating the screens when resuming from being tombstoned.
    /// </summary>
    public class ScreenFactory : IScreenFactory
    {
        public GameScreen CreateScreen(Type screenType)
        {
            return Activator.CreateInstance(screenType) as GameScreen
                ?? throw new ArgumentException($"Unknown screen type {screenType}");
        }
    }
}
