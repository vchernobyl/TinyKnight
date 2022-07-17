using System;

namespace Gravity
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new GravityGame();
            game.Run();
        }
    }
}
