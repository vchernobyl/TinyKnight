using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TinyKnight
{
    /// <summary>
    /// Defines an action that is designed by some set of buttons
    /// and/or keys.
    /// 
    /// The way actions work is that you define a set of buttons
    /// and keys that trigger the action. You can then evaluate the
    /// action against an InputState which will test to see if any
    /// of the buttons or keys are pressed by a player. You can also
    /// set a flag that indicates if the action only occurs once when
    /// the buttons/keys are first pressed or whether the actions
    /// should occur each frame.
    /// 
    /// Using this InputAction class means that you can configure
    /// new actions based on keys and buttons without having to directly
    /// modify the InputState type. This means more customization by
    /// your games without having to change the core classes of the
    /// Game State Management.
    /// </summary>
    public class InputAction
    {
        private readonly Buttons[] buttons;
        private readonly Keys[] keys;
        private readonly bool newPressOnly;

        // These delegate types map to the methods on InputState.
        // We use these to simplify the evaluate method by allowing
        // us to map the appropriate delegates and invoke them, rather
        // than having two separate code paths.
        private delegate bool ButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex player);
        private delegate bool KeyPress(Keys button, PlayerIndex? controllingPlayer, out PlayerIndex player);

        public InputAction(Buttons[] buttons, Keys[] keys, bool newPressOnly)
        {
            this.buttons = buttons;
            this.keys = keys;
            this.newPressOnly = newPressOnly;
        }

        public bool Evaluate(InputState input)
        {
            return Evaluate(input, null, out _);
        }

        public bool Evaluate(InputState state, PlayerIndex? controllingPlayer, out PlayerIndex player)
        {
            // Figure out which delegate methods to map from the state
            // which takes care of out "newPressOnly" logic.
            ButtonPress buttonTest;
            KeyPress keyTest;
            if (newPressOnly)
            {
                buttonTest = state.IsNewButtonPress;
                keyTest = state.IsNewKeyPress;
            }
            else
            {
                buttonTest = state.IsButtonPressed;
                keyTest = state.IsKeyPressed;
            }

            foreach (var button in buttons)
            {
                if (buttonTest(button, controllingPlayer, out player))
                    return true;
            }

            foreach (var key in keys)
            {
                if (keyTest(key, controllingPlayer, out player))
                    return true;
            }

            // If we got here, the action is not matched.
            player = PlayerIndex.One;
            return false;
        }
    }
}
