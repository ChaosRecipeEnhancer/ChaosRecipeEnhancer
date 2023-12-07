using System;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.Models.Hotkeys;

/// <summary>
///     A hotkey containing a key combination (a key and modifier key) and a
///     callback function that gets called if the right keys are down. Also contains a
///     boolean stating if the callback method can be called or not.
/// </summary>
public class GlobalHotkey
{
    /// <summary>
    ///     Initiates a new hotkey with the given modifier, key, callback method,
    ///     and also a boolean stating if the callback can be run (can be changed, see <see cref="CanExecute" />)
    /// </summary>
    /// <param name="modifier">The modifier key required to be pressed</param>
    /// <param name="key">The key required to be pressed</param>
    /// <param name="callbackMethod">The method that gets called when the hotkey is fired</param>
    /// <param name="cooldown">The cooldown of the hotkey.</param>
    /// <param name="lastInvoked">The last time the hotkey was invoked.</param>
    /// <param name="canExecute">States whether the callback can be run (can be changed, see <see cref="CanExecute" />)</param>
    public GlobalHotkey(ModifierKeys modifier, Key key, Action callbackMethod, TimeSpan cooldown, DateTime lastInvoked, bool canExecute = true)
    {
        Modifier = modifier;
        Key = key;
        Callback = callbackMethod;
        Cooldown = cooldown;
        LastInvoked = lastInvoked;
        CanExecute = canExecute;
    }

    /// <summary>
    ///     The modifier key required to be pressed for the hotkey to be
    /// </summary>
    public ModifierKeys Modifier { get; set; }

    /// <summary>
    ///     The key required to be pressed for the hotkey to be fired
    /// </summary>
    public Key Key { get; set; }

    /// <summary>
    ///     The method to be called when the hotkey is pressed.
    ///     You could change this to a list of actions if you
    ///     want multiple things to be fired when the hotkey fires.
    /// </summary>
    public Action Callback { get; set; }

    /// <summary>
    ///     States if the method can be executed.
    /// </summary>
    public bool CanExecute { get; set; }

    /// <summary>
    ///    The cooldown of the hotkey.
    /// </summary>
    public TimeSpan Cooldown { get; set; }

    /// <summary>
    ///    The last time the hotkey was invoked.
    /// </summary>
    public DateTime LastInvoked { get; set; }
}