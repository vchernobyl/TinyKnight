using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gravity
{
    /// <summary>
    /// The screen manager is a component which manages one or more
    /// GameScreen instances. It maintains a stack of screens, calls
    /// their Update and Draw methods at appropriate time, and
    /// automatically routes input to the topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields
        readonly List<GameScreen> screens = new();
        readonly List<GameScreen> tempScreenList = new();
        readonly InputState input = new();

        bool isInitialized;
        #endregion

        #region Properties
        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saved
        /// each screen having to bother creating theor own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// A default font shared by all the screens. This saves
        /// each screen having to bother loading their own local copy.
        /// </summary>
        public SpriteFont Font { get; private set; }

        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled { get; set; }

        public Texture2D BlankTexture { get; private set; }
        #endregion

        public ScreenManager(Game game) : base(game)
        {
            TouchPanel.EnabledGestures = GestureType.None;
        }

        public override void Initialize()
        {
            base.Initialize();
            isInitialized = true;
        }

        protected override void LoadContent()
        {
            var content = Game.Content;
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Font = content.Load<SpriteFont>("Fonts/Default");
            BlankTexture = content.Load<Texture2D>("Textures/Blank");

            foreach (var screen in screens)
                screen.Activate(instancePreserved: false);
        }

        protected override void UnloadContent()
        {
            foreach (var screen in screens)
                screen.Unload();
        }

        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            input.Update();

            // Make a copy of the master screen list, to avoid
            // confusion if the process of updating one screen
            // adds or removes others.
            tempScreenList.Clear();

            foreach (var screen in screens)
                tempScreenList.Add(screen);

            var otherScreenHasFocus = !Game.IsActive;
            var coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (tempScreenList.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                var screen = tempScreenList[^1];
                tempScreenList.RemoveAt(tempScreenList.Count - 1);

                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(gameTime, input);
                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            if (TraceEnabled)
                TraceScreens();
        }

        void TraceScreens()
        {
            var screenNames = new List<string>();
            foreach (var screen in screens)
                screenNames.Add(screen.GetType().Name);

            Debug.WriteLine(string.Join(separator: ", ", screenNames.ToArray()));
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }

        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (isInitialized)
                screen.Activate(instancePreserved: false);

            screens.Add(screen);

            // Update the TouchPanel to respond to the gestures this screen
            // is interested in.
            TouchPanel.EnabledGestures = screen.EnabledGestures;
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScree instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        /// <param name="gameScreen"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (isInitialized)
                screen.Unload();

            screens.Remove(screen);
            tempScreenList.Remove(screen);

            // If there is a screen still in the manager, update TouchPanel
            // to respond to gestures that screen is interested in.
            if (screens.Count > 0)
                TouchPanel.EnabledGestures = screens[^1].EnabledGestures;
        }

        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }

        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// the screens in and out, and for darkening the background behind popups.
        /// </summary>
        /// <param name="alpha"></param>
        public void FadeBackBufferToBlack(float alpha)
        {
            SpriteBatch.Begin();
            SpriteBatch.Draw(BlankTexture, GraphicsDevice.Viewport.Bounds, Color.Black * alpha);
            SpriteBatch.End();
        }

        /// <summary>
        /// Informs the screen manager to serialize its state to disk.
        /// </summary>
        public void Deactivate()
        {
            throw new NotImplementedException();
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }
    }
}
