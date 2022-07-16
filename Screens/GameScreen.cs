using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace Gravity
{
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    public abstract class GameScreen
    {
        public bool IsPopup { get; protected set; }

        public TimeSpan TransitionOnTime { get; protected set; } = TimeSpan.Zero;

        public TimeSpan TransitionOffTime { get; protected set; } = TimeSpan.Zero;

        public float TransitionPosition { get; protected set; } = 1f;

        public float TransitionAlpha => 1f - TransitionPosition;

        public ScreenState ScreenState { get; protected set; } = ScreenState.TransitionOn;

        public bool IsExiting { get; protected internal set; }

        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus &&
                    (ScreenState == ScreenState.TransitionOn ||
                     ScreenState == ScreenState.Active);
            }
        }

        bool otherScreenHasFocus;

        public ScreenManager ScreenManager { get; internal set; }

        /// <summary>
        /// Gets the index of the player who is currently controlling
        /// this screen, or null if it is accepting input from any player.
        /// This is used to lock the game to a specific player profile.
        /// The main menu responds to input from any connected gamepad,
        /// but whichever player makes a selection from this menu is given
        /// control over all subsequent screens, so other gamepads are
        /// inactive until the controlling player returns to the main menu.
        /// </summary>
        public PlayerIndex? ControllingPlayer { get; internal set; }

        GestureType enabledGestures = GestureType.None;
        public GestureType EnabledGestures
        {
            get { return enabledGestures; }
            protected set
            {
                enabledGestures = value;

                // The screen manager handles this during screen changes,
                // but if this screen is active and the gesture types are
                // changing, we have to update the TouchPanel ourself.
                if (ScreenState == ScreenState.Active)
                    TouchPanel.EnabledGestures = value;
            }
        }

        public bool IsSerializable { get; protected set; } = true;

        /// <summary>
        /// Activates the screen. Called when the screen is added to the screen
        /// manager or if the game resumes from being paused or tombstoned.
        /// </summary>
        /// <param name="instancePreserved">
        /// True if the game was preserved during deactivation, false if the
        /// screen is just being added or if the game was tombstoned.
        /// On Xbox and Windows this will always be false.
        /// </param>
        public virtual void LoadContent() { }

        public virtual void UnloadContent() { }

        public virtual void Update(GameTime gameTime,
            bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (IsExiting)
            {
                ScreenState = ScreenState.TransitionOff;
                if (!UpdateTransition(gameTime, TransitionOffTime, 1))
                    ScreenManager.RemoveScreen(this);
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (UpdateTransition(gameTime, TransitionOffTime, 1))
                {
                    // Still busy transitioning.
                    ScreenState = ScreenState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    ScreenState = ScreenState.Hidden;
                }
            }
            else
            {
                if (UpdateTransition(gameTime, TransitionOnTime, -1))
                {
                    // Still busy transitioning.
                    ScreenState = ScreenState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    ScreenState = ScreenState.Active;
                }
            }
        }

        bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            var transitionDelta = time == TimeSpan.Zero
                ? 1f
                : (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            TransitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (direction < 0 && TransitionPosition <= 0 ||
                direction > 0 && TransitionPosition >= 1)
            {
                TransitionPosition = MathHelper.Clamp(TransitionPosition, 0f, 1f);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }

        // Allows the screen to handle user input. Unlike Update, this method
        // is only called when the screen is active, and not when some other
        // screen has taken the focus.
        public virtual void HandleInput(GameTime gameTime, InputState input) { }

        public virtual void Draw(GameTime gameTime) { }

        // Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        // instantly kills the screen, this method respects the transition timings
        // and will gie the screen a chance to gradually transition off.
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
                ScreenManager.RemoveScreen(this);
            else
                IsExiting = true;
        }
    }
}
