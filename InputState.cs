﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace Gravity
{
    // TODO: Currently we support 4 gamepads at once, which is cool.
    // However, we technically also implement support for 4 connected
    // keyboard simultaneously, which is not needed and I'm also sure
    // it's not going to work.
    public class InputState
    {
        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;

        public readonly bool[] GamePadWasConnected;

        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();

        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];

            GamePadWasConnected = new bool[MaxInputs];
        }

        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState();
                CurrentGamePadStates[i] = GamePad.GetState(i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (CurrentGamePadStates[i].IsConnected)
                    GamePadWasConnected[i] = true;
            }

            // Get the raw touch state.
            TouchState = TouchPanel.GetState();

            // Read in any detected gestures into our list for the screens
            // to later process.
            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
                Gestures.Add(TouchPanel.ReadGesture());
        }

        /// <summary>
        /// Helper for checking if a key was pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsKeyPressed(Keys key, PlayerIndex? controllingPlayer,
            out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;
                var i = (int)playerIndex;
                return CurrentKeyboardStates[i].IsKeyDown(key);
            }

            return IsKeyPressed(key, PlayerIndex.One, out playerIndex) ||
                   IsKeyPressed(key, PlayerIndex.Two, out playerIndex) ||
                   IsKeyPressed(key, PlayerIndex.Three, out playerIndex) ||
                   IsKeyPressed(key, PlayerIndex.Four, out playerIndex);
        }

        public bool IsButtonPressed(Buttons button, PlayerIndex? controllingPlayer,
            out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;
                var i = (int)playerIndex;
                return CurrentGamePadStates[i].IsButtonDown(button);
            }

            return IsButtonPressed(button, PlayerIndex.One, out playerIndex) ||
                   IsButtonPressed(button, PlayerIndex.Two, out playerIndex) ||
                   IsButtonPressed(button, PlayerIndex.Three, out playerIndex) ||
                   IsButtonPressed(button, PlayerIndex.Four, out playerIndex);
        }

        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer,
            out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;
                var i = (int)playerIndex;
                return CurrentKeyboardStates[i].IsKeyDown(key) &&
                       LastKeyboardStates[i].IsKeyUp(key);
            }

            return IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                   IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                   IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                   IsNewKeyPress(key, PlayerIndex.Four, out playerIndex);
        }

        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer,
            out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;
                var i = (int)playerIndex;
                return CurrentGamePadStates[i].IsButtonDown(button) &&
                       LastGamePadStates[i].IsButtonUp(button);
            }

            return IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                   IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                   IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                   IsNewButtonPress(button, PlayerIndex.Four, out playerIndex);
        }
    }
}