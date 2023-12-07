using ChaosRecipeEnhancer.UI.Models.Hotkeys;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using ChaosRecipeEnhancer.UI.State;

namespace ChaosRecipeEnhancer.UI.Utilities.Native;

internal static class KeyboardHookForGlobalHotkeys
{
    /// <summary>
    ///     States whether the system keyboard event hook is setup.
    /// </summary>
    public static bool IsHookSetup { get; private set; }

    // Events
    public delegate void HotkeyEvent(GlobalHotkey hotkey);

    // The build in proc ID for telling windows to hook onto the
    // low level keyboard events with the SetWindowsHookEx function
    private const int WH_KEYBOARD_LL = 13;
    private static readonly LowLevelKeyboardProc LowLevelProc = HookCallback;

    // The system hook ID (for storing this application's hook)
    private static IntPtr HookID = IntPtr.Zero;

    // Callbacks
    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    ///     Fired when a hotkey is fired (duh lol).
    /// </summary>
    public static event HotkeyEvent HotkeyFired;

    /// <summary>
    ///     Hooks/Sets up this application for receiving keydown callbacks
    /// </summary>
    public static void SetupSystemHook()
    {
        HookID = SetHook(LowLevelProc);
        IsHookSetup = true;
    }

    /// <summary>
    ///     Unhooks this application, stopping it from receiving keydown callbacks
    /// </summary>
    public static void ShutdownSystemHook()
    {
        UnhookWindowsHookEx(HookID);
        IsHookSetup = false;
    }

    /// <summary>
    ///     Sets up the Key Up/Down event hooks.
    /// </summary>
    /// <param name="proc">The callback method to be called when a key up/down occurs</param>
    /// <returns></returns>
    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (var curProcess = Process.GetCurrentProcess())
        {
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
    }

    /// <summary>
    ///     The method called when a key up/down occurs across the system.
    /// </summary>
    /// <param name="nCode">idk tbh</param>
    /// <param name="wParam"></param>
    /// <param name="lParam">LPARAM, contains the key that was pressed. not used atm</param>
    /// <returns>LRESULT</returns>
    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        // Checks if this is called from keydown only because key ups aren't used.
        if (nCode >= 0)
            CheckHotkeys();

        // Cannot use System.Windows' keys because
        // they dont use the same values as windows
        //int vkCode = Marshal.ReadInt32(lParam);
        //System.Windows.Forms.Keys key = (System.Windows.Forms.Keys)vkCode;
        //Debug.WriteLine(key);

        // I think this tells windows that this app has successfully
        // handled the key events and now other apps can handle it next.
        return CallNextHookEx(HookID, nCode, wParam, lParam);
    }

    /// <summary>
    ///     Checks if there are any modifiers are pressed. If so, it checks through every
    ///     Hotkey and matches their Modifier/Key. If they both match, and the hotkey allows
    ///     the callback method to be called, it is called.
    /// </summary>
    private static void CheckHotkeys()
    {
        var now = DateTime.Now;

        foreach (var dictionaryEntry in GlobalHotkeyState.Hotkeys)
        {
            var hotkey = dictionaryEntry.Value;
            if (hotkey.Key != Key.None && Keyboard.Modifiers == hotkey.Modifier && Keyboard.IsKeyDown(hotkey.Key))
            {
                if (now - hotkey.LastInvoked > hotkey.Cooldown)
                {
                    if (hotkey.CanExecute)
                    {
                        hotkey.Callback?.Invoke();
                        HotkeyFired?.Invoke(hotkey);
                        hotkey.LastInvoked = now;
                    }
                }
            }
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}

