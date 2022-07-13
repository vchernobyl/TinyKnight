using Microsoft.Xna.Framework;
using System;

namespace Gravity
{
    class MainMenuScreen : MenuScreen
    {
        public MainMenuScreen() : base(menuTitle: "Main Menu")
        {
            var playGameMenuEntry = new MenuEntry("Play Game");
            var optionsMenuEntry = new MenuEntry("Options");
            var exitMenuEntry = new MenuEntry("Exit");

            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            var message = "Are you sure you want to exit the game?";
            var confirmExitMessageBox = new MessageBoxScreen(message);
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        void ConfirmExitMessageBoxAccepted(object? sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        void OptionsMenuEntrySelected(object? sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        void PlayGameMenuEntrySelected(object? sender, PlayerIndexEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
