using System;

namespace TinyKnight
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new TinyKnightGame();
            game.Run();
        }
    }
}
