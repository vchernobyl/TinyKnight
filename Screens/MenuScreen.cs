using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TinyKnight
{
    /// <summary>
    /// Base class for screens that contain a menu of options.
    /// The user can move up and down to select an entry, or
    /// cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        #region Fields

        readonly List<MenuEntry> menuEntries = new List<MenuEntry>();
        readonly string menuTitle;

        readonly InputAction menuUp;
        readonly InputAction menuDown;
        readonly InputAction menuSelect;
        readonly InputAction menuCancel;

        int selectedEntry = 0;

        #endregion

        #region Properties

        protected IList<MenuEntry> MenuEntries => menuEntries;

        #endregion

        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(.5);
            TransitionOffTime = TimeSpan.FromSeconds(.5);

            menuUp = new InputAction(
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new Keys[] { Keys.Up },
                newPressOnly: true);

            menuDown = new InputAction(
                new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new Keys[] { Keys.Down },
                newPressOnly: true);

            menuSelect = new InputAction(
                new Buttons[] { Buttons.A, Buttons.Start },
                new Keys[] { Keys.Enter, Keys.Space },
                newPressOnly: true);

            menuCancel = new InputAction(
                new Buttons[] { Buttons.B, Buttons.Back },
                new Keys[] { Keys.Escape },
                newPressOnly: true);
        }

        /// <summary>
        /// Responds to the user input, changin the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            // For input tests we pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or specific index.
            // If we pass a null controlling player, the InputState helper return
            // to us which player actually provided the input. We pass that through
            // to OnSelectedEntry and OnCancel, so they can tell which player
            // triggered them.
            PlayerIndex playerIndex;

            if (menuUp.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry--;
                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            if (menuDown.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry++;
                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            if (menuSelect.Evaluate(input, ControllingPlayer, out playerIndex))
                OnSelectEntry(selectedEntry, playerIndex);
            else if (menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
                OnCancel(playerIndex);
        }

        protected void OnSelectEntry(int selectedEntry, PlayerIndex playerIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry(playerIndex);
        }

        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }

        protected void OnCancel(object? sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries.
        /// By default all menu entries are lines up in a vertical list,
        /// centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transition, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            var transitionOffset = MathF.Pow(TransitionPosition, 2f);

            // Start at Y = 175, each X value is generated per entry.
            var position = new Vector2(0f, 175f);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var menuEntry = menuEntries[i];
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;

                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                menuEntry.Position = position;

                // Move down for the next entry the size of this entry.
                position.Y += menuEntry.GetHeight(this);
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var isSelected = IsActive && i == selectedEntry;
                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            UpdateMenuEntryLocations();

            var graphics = ScreenManager.GraphicsDevice;
            var spriteBatch = ScreenManager.SpriteBatch;
            var font = ScreenManager.Font;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                var menuEntry = menuEntries[i];
                var isSelected = IsActive && i == selectedEntry;
                menuEntry.Draw(this, isSelected, gameTime);
            }

            // Make the menu slide into place during transition, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            var transitionOffset = MathF.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen.
            var titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            var titleOrigin = font.MeasureString(menuTitle) / 2;
            var titleColor = new Color(192, 192, 192) * TransitionAlpha;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor,
                rotation: 0f, titleOrigin, scale: 1.25f, SpriteEffects.None, layerDepth: 0);

            spriteBatch.End();
        }
    }
}
