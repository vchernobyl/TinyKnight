using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TinyKnight
{
    /// <summary>
    /// Helper class that represents a single entry in a MenuScreen.
    /// By default this just draws the entry text string, but it
    /// can be customized to display menu entries in different ways.
    /// This also provides an event that will be raised when the
    /// menu entry is selected.
    /// </summary>
    class MenuEntry
    {
        #region Fields
        float selectionFade;
        #endregion

        #region Properties
        public string Text { get; set; }
        public Vector2 Position { get; set; }
        #endregion

        #region Events
        public event EventHandler<PlayerIndexEventArgs>? Selected;

        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            Selected?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
        }
        #endregion

        public MenuEntry(string text)
        {
            Text = text;
        }

        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            var fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4f;
            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }

        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            var color = isSelected ? Color.Yellow : Color.White;
            var time = gameTime.TotalGameTime.TotalSeconds;
            var pulsate = (float)Math.Sin(time * 6) + 1;
            var scale = 1 + pulsate * .05f * selectionFade;

            // Modify the alpha to fade the text out during transition.
            color *= screen.TransitionAlpha;

            // Draw text, cented on the middle of each line.
            var screenManager = screen.ScreenManager;
            var spriteBatch = screenManager.SpriteBatch;
            var font = screenManager.Font;

            var origin = new Vector2(0f, font.LineSpacing / 2f);

            spriteBatch.DrawString(font, Text, Position, color, 0f,
                origin, scale, SpriteEffects.None, 0f);
        }

        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.ScreenManager.Font.LineSpacing;
        }

        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.ScreenManager.Font.MeasureString(Text).X;
        }
    }
}