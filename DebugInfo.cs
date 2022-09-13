using Microsoft.Xna.Framework.Input;

namespace Gravity
{
    public static class DebugInfo
    {
        public static bool ShowSolids { get; private set; }
        public static bool ShowNavigation { get; private set; }
        public static bool ShowEntityColliders { get; private set; }

        public static void HandleInput()
        {
#if DEBUG
            if (Input.WasKeyPressed(Keys.S))
                ShowSolids = !ShowSolids;
            if (Input.WasKeyPressed(Keys.E))
                ShowEntityColliders = !ShowEntityColliders;
            if (Input.WasKeyPressed(Keys.N))
                ShowNavigation = !ShowNavigation;
#endif
        }
    }
}
