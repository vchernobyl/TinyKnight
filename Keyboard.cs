using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public class Keyboard
    {
        private static KeyboardState current;
        private static KeyboardState previous;

        public static KeyboardState GetState()
        {
            previous = current;
            current = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            return current;
        }

        public static bool IsKeyDown(Keys key)
        {
            return current.IsKeyDown(key);
        }

        public static bool WasKeyPressed(Keys key)
        {
            return current.IsKeyDown(key) && !previous.IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return current.IsKeyUp(key);
        }
    }
}
