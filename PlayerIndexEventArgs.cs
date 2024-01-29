using Microsoft.Xna.Framework;
using System;

namespace TinyKnight
{
    class PlayerIndexEventArgs : EventArgs
    {
        public PlayerIndex PlayerIndex { get; }

        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}