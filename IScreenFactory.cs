﻿using System;

namespace Gravity
{
    /// <summary>
    /// Defines an object that can create a screen given its type.
    /// 
    /// The ScreenManager attempts to handle tombstoning (killing of the process)
    /// on Windows Phone by creating an XML document that has a list of the screens
    /// currently in the manager. When the game is reactivated, the ScreenManager
    /// needs to create instances of those screens. However, since there is no
    /// restriction that a particular GameScreen subclass has a prameterless
    /// constructor, there is no way the Screenmanager alone could create those
    /// instances.
    /// 
    /// IScreenFactory fills this gap by providing an interface the game should
    /// implement to act as a translation from type to instance. The ScreenManager
    /// locates the IScreenFactory from the Game.Services collection and passes
    /// each screen type to the factory, expecting to get the correct GameScreen
    /// out.
    /// 
    /// If your game screens all have parameterless constructors, the minimal
    /// implementation of this interface would look like this:
    /// 
    ///     return Activator.CreateInstance(screenType) as GameScreen;
    /// 
    /// If you have screens with constructors that take arguments, you will need
    /// to ensure that you can read those arguments from storage or generate new
    /// ones, then construct the screen based on the type.
    /// 
    /// The ScreenFactory type in the sample game has the minimal implementation
    /// along with some extra comments showing potentially more complex example
    /// of how to implement IScreenFactory.
    /// </summary>
    public interface IScreenFactory
    {
        GameScreen CreateScreen(Type screenType);
    }
}
