using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class Input
    {
        private static KeyboardState current;
        private static KeyboardState previous;

        public static bool Enabled = true;

        public static KeyboardState GetState()
        {
            previous = current;
            current = Keyboard.GetState();
            return current;
        }

        public static bool IsKeyDown(Keys key)
        {
            return Enabled && current.IsKeyDown(key);
        }

        public static bool WasKeyPressed(Keys key)
        {
            return Enabled && current.IsKeyDown(key) && !previous.IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return Enabled && current.IsKeyUp(key);
        }
    }
}
