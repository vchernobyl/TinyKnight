namespace Gravity
{
    internal class PauseMenuScreen : MenuScreen
    {
        public PauseMenuScreen() : base(menuTitle: "Paused")
        {
            var resumeEntry = new MenuEntry("Resume Game");
            var quitEntry = new MenuEntry("Quit Game");

            resumeEntry.Selected += OnCancel;
            quitEntry.Selected += QuitGameSelected;
        }

        private void QuitGameSelected(object? sender, PlayerIndexEventArgs e)
        {
            var message = "Are you sure you want to quit the game?";
            var confirmMessageBox = new MessageBoxScreen(message);
            confirmMessageBox.Accepted += ConfirmQuiteMessageBoxAccepted;
            ScreenManager.AddScreen(confirmMessageBox, ControllingPlayer);
        }

        private void ConfirmQuiteMessageBoxAccepted(object? sender, PlayerIndexEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}