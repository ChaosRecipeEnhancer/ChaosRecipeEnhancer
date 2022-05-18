using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using EnhancePoE.UI.Properties;
using Serilog;

namespace EnhancePoE.UI
{
    /// <summary>
    ///     A class for adding/removing global hotkeys to and from your application,
    ///     meaning these hotkeys can be run even if your application isn't focused.
    /// </summary>
    public static class HotkeysManager
    {
        // Events

        public delegate void HotkeyEvent(GlobalHotkey hotkey);

        // The build in proc ID for telling windows to hook onto the
        // low level keyboard events with the SetWindowsHookEx function
        private const int WH_KEYBOARD_LL = 13;
        private static readonly LowLevelKeyboardProc LowLevelProc = HookCallback;

        // The system hook ID (for storing this application's hook)
        private static IntPtr HookID = IntPtr.Zero;

        public static ModifierKeys refreshModifier;
        public static List<ModifierKeys> refreshModifiers;
        public static Key refreshKey;

        public static ModifierKeys toggleModifier;
        public static List<ModifierKeys> toggleModifiers;
        public static Key toggleKey;

        public static ModifierKeys stashTabModifier;
        public static List<ModifierKeys> stashTabModifiers;
        public static Key stashTabKey;

        private static readonly ILogger _logger;

        //public static ModifierKeys reloadFilterModifier;
        //public static Key reloadFilterKey;

        private static bool isPressed { get; set; }

        static HotkeysManager()
        {
            _logger = Log.Logger;
            Hotkeys = new List<GlobalHotkey>();
            if (toggleModifiers == null)
                toggleModifiers = new List<ModifierKeys>();
            RequiresModifierKey = false;
        }

        // All of the Hotkeys
        private static List<GlobalHotkey> Hotkeys { get; set; }

        /// <summary>
        ///     States whether the system keyboard event hook is setup.
        /// </summary>
        public static bool IsHookSetup { get; private set; }

        /// <summary>
        ///     States whether hotkeys require modifier keys to be scanned (and therefore
        ///     have a chance for their callback method to be called). If this is disabled,
        ///     the hotkeys will be checked in every key stroke/event, so pressing just 'A' could
        ///     fire a hotkey if there is one with no modifiers and just it's key set to 'A'.
        ///     <para>
        ///         If enabled, a modifier key is required on hotkeys. if the hotkey
        ///         has no modifiers, then it simply wont be scanned at all.
        ///     </para>
        /// </summary>
        public static bool RequiresModifierKey { get; set; }

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
            _logger.Debug("System hooks setup successfully");
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
        ///     Adds a hotkey to the hotkeys list.
        /// </summary>
        public static void AddHotkey(GlobalHotkey hotkey)
        {
            Hotkeys.Add(hotkey);
        }

        /// <summary>
        ///     Removes a hotkey from the hotkeys list.
        /// </summary>
        /// <param name="hotkey"></param>
        public static void RemoveHotkey(GlobalHotkey hotkey)
        {
            Hotkeys.Remove(hotkey);
        }

        /// <summary>
        /// Its self explanatory.
        /// </summary>
        /// <param name="hotkey">all the information needed for this too function lul</param>
        public static void HandleKey(GlobalHotkey hotkey)
        {
            if(Keyboard.IsKeyDown(hotkey.Key) && !isPressed)
            {
                isPressed = true;
                if(hotkey.CanExecute)
                    hotkey.Callback?.Invoke();
            }
        }

        /// <summary>
        ///     Checks if there are any modifiers are pressed. If so, it checks through every
        ///     Hotkey and matches their Modifier/Key. If they both match, and the hotkey allows
        ///     the callback method to be called, it is called.
        /// </summary>
        private static void CheckHotkeys()
        {
            foreach (var hotkey in Hotkeys)
            {
                if (hotkey.ReqModifiers)
                {
                    if (hotkey.Modifiers != null && hotkey.Modifiers.Count > 1)
                    {
                        var ctrl_alt_shift = Keyboard.Modifiers.HasFlag(ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift) &&
                            hotkey.Modifiers.Contains(ModifierKeys.Control) &&
                            hotkey.Modifiers.Contains(ModifierKeys.Shift) &&
                            hotkey.Modifiers.Contains(ModifierKeys.Alt);
                        var ctrl_alt = Keyboard.Modifiers.HasFlag(ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift) &&
                            hotkey.Modifiers.Contains(ModifierKeys.Control) &&
                            hotkey.Modifiers.Contains(ModifierKeys.Alt) && 
                            !hotkey.Modifiers.Contains(ModifierKeys.Shift);
                        var ctrl_shift = Keyboard.Modifiers.HasFlag(ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift) &&
                            hotkey.Modifiers.Contains(ModifierKeys.Control) &&
                            hotkey.Modifiers.Contains(ModifierKeys.Shift) && 
                            !hotkey.Modifiers.Contains(ModifierKeys.Alt);
                        var shift_alt = Keyboard.Modifiers.HasFlag(ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift) &&
                            hotkey.Modifiers.Contains(ModifierKeys.Shift) &&
                            hotkey.Modifiers.Contains(ModifierKeys.Alt) && 
                            !hotkey.Modifiers.Contains(ModifierKeys.Control);

                        if(hotkey.Key != Key.None)
                        {
                            if(ctrl_alt_shift)
                            {
                                HandleKey(hotkey);
                            }
                            else if (ctrl_alt)
                            {
                                HandleKey(hotkey);
                            }
                            else if (ctrl_shift)
                            {
                                HandleKey(hotkey);
                            }
                            else if (shift_alt)
                            {
                                HandleKey(hotkey);
                            }

                            if (Keyboard.IsKeyUp(hotkey.Key) && isPressed)
                            {
                                isPressed = false;
                            }
                        }
                    }
                    
                    if (hotkey.Modifiers == null && hotkey.Modifier != ModifierKeys.None)
                    {
                        var alt =
                            Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) &&
                            hotkey.Modifier.HasFlag(ModifierKeys.Alt) &&
                            !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) &&
                            !Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
                        var control =
                            Keyboard.Modifiers.HasFlag(ModifierKeys.Control) &&
                            hotkey.Modifier.HasFlag(ModifierKeys.Control) &&
                            !Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) &&
                            !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
                        var shift =
                            Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) &&
                            hotkey.Modifier.HasFlag(ModifierKeys.Shift) &&
                            !Keyboard.Modifiers.HasFlag(ModifierKeys.Alt) &&
                            !Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

                        if(hotkey.Key != Key.None)
                        {
                            if (alt)
                            {
                                HandleKey(hotkey);
                            }
                            else if (control)
                            {
                                HandleKey(hotkey);
                            }
                            else if (shift)
                            {
                                HandleKey(hotkey);
                            }
                            if (Keyboard.IsKeyUp(hotkey.Key) && isPressed)
                            {
                                isPressed = false;
                            }
                        }
                    }
                }

                if (!hotkey.ReqModifiers && Keyboard.Modifiers == ModifierKeys.None)
                {
                    if (hotkey.Key != Key.None)
                    {
                        HandleKey(hotkey);

                        if (Keyboard.IsKeyUp(hotkey.Key) && isPressed)
                        {
                            isPressed = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Finds and returns all hotkeys in the hotkeys list that have matching modifiers and keys given
        /// </summary>
        /// <param name="modifier"></param>
        /// <param name="key"></param>
        /// <param name="callbackMethod">If this is not null, the callback method will be checked</param>
        /// <returns></returns>
        public static List<GlobalHotkey> FindHotkeys(ModifierKeys modifier, Key key)
        {
            var hotkeys = new List<GlobalHotkey>();
            foreach (var hotkey in Hotkeys)
                if (hotkey.Key == key && hotkey.Modifier == modifier)
                    hotkeys.Add(hotkey);

            return hotkeys;
        }
        /// <summary>
        ///     Finds and returns all hotkeys in the hotkeys list that have matching modifiers and keys given
        /// </summary>
        /// <param name="modifier"></param>
        /// <param name="key"></param>
        /// <param name="callbackMethod">If this is not null, the callback method will be checked</param>
        /// <returns></returns>
        public static List<GlobalHotkey> FindHotkeys(List<ModifierKeys> modifiers, Key key)
        {
            var hotkeys = new List<GlobalHotkey>();
            foreach (var hotkey in Hotkeys)
                if (hotkey.Key == key && hotkey.Modifiers == modifiers)
                    hotkeys.Add(hotkey);

            return hotkeys;
        }

        /// <summary>
        ///     Creates and adds a new hotkey to the hotkeys list.
        /// </summary>
        /// <param name="modifier">Single modifier ALT,CTRL,SHIFT</param>
        /// <param name="key"></param>
        /// <param name="callbackMethod"></param>
        /// <param name="canExecute"></param>
        public static void AddHotkey(ModifierKeys modifier, Key key, Action callbackMethod,bool reqModifiers = false, bool canExecute = true)
        {
            if(modifier != ModifierKeys.None) reqModifiers = true;
            AddHotkey(new GlobalHotkey(modifier, key, callbackMethod, reqModifiers, canExecute));
        }
        /// <summary>
        ///     Creates and adds a new hotkey to the hotkeys list.
        /// </summary>
        /// <param name="modifier">Can use any combinations of modifiers out of CTRL+ALT+SHIFT for now.</param>
        /// <param name="key"></param>
        /// <param name="callbackMethod"></param>
        /// <param name="canExecute"></param>
        public static void AddHotkey(List<ModifierKeys> modifiers, Key key, Action callbackMethod, bool canExecute = true)
        {
            AddHotkey(new GlobalHotkey(modifiers, key, callbackMethod, canExecute));
        }

        /// <summary>
        ///     Removes a or all hotkey from the hotkeys list (depending on
        ///     <paramref name="removeAllOccourances" />) by going through every hotkey
        ///     and checking it's modifier and key to see if they match. is so, it removes it.
        /// </summary>
        /// <param name="modifier"></param>
        /// <param name="key"></param>
        /// <param name="removeAllOccourances">
        ///     If this is false, the first found hotkey will be removed.
        ///     else, every occourance will be removed.
        /// </param>
        public static void RemoveHotkey(ModifierKeys modifier, Key key, bool removeAllOccourances = false)
        {
            var originalHotkeys = Hotkeys;
            var toBeRemoved = FindHotkeys(modifier, key);

            if (toBeRemoved.Count > 0)
            {
                if (removeAllOccourances)
                {
                    foreach (var hotkey in toBeRemoved) originalHotkeys.Remove(hotkey);

                    Hotkeys = originalHotkeys;
                }
                else
                {
                    RemoveHotkey(toBeRemoved[0]);
                }
            }
        }
        public static void RemoveHotkey(List<ModifierKeys> modifiers, Key key, bool removeAllOccourances = false)
        {
            var originalHotkeys = Hotkeys;
            var toBeRemoved = FindHotkeys(modifiers, key);

            if (toBeRemoved.Count > 0)
            {
                if (removeAllOccourances)
                {
                    foreach (var hotkey in toBeRemoved) originalHotkeys.Remove(hotkey);

                    Hotkeys = originalHotkeys;
                }
                else
                {
                    RemoveHotkey(toBeRemoved[0]);
                }
            }
        }

        /// <summary>
        ///     Sets up the Key Up/Down event hooks.
        /// </summary>
        /// <param name="proc">The callback method to be called when a key up/down occours</param>
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
        ///     The method called when a key up/down occours across the system.
        /// </summary>
        /// <param name="nCode">idk tbh</param>
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
        /// <para>
        /// This method will refresh the fields with a new key and modifier(s).
        /// </para>
        /// <example>
        /// Example:
        /// <code>
        /// RefreshHotKey("CTRL + ALT + SHIFT + F1")
        /// </code>
        /// Sets corresponding fields with correct key and modifier(s).
        /// </example>
        /// </summary>
        /// <param name="hotkey">the string containing a new key and modifier(s).</param>
        public static void RefreshHotkey(string hotkey)
        {
            if (hotkey != "< not set >")
            {
                string[] split_string = hotkey.Split('+');
                
                if (split_string.Length > 1 && split_string.Length < 3)
                {
                    string compare_string = split_string[0].Trim().ToLower();
                    ModifierKeys temp_key;
                    if (compare_string.Equals("ctrl"))
                    {
                        temp_key = ModifierKeys.Control;
                    }
                    else if (compare_string.Equals("alt"))
                    {
                        temp_key = ModifierKeys.Alt;
                    }
                    else if (compare_string.Equals("shift"))
                    {
                        temp_key = ModifierKeys.Shift;
                    }
                    else if (compare_string.Equals("win"))
                    {
                        temp_key = ModifierKeys.Windows;
                    }
                    else
                    {
                        temp_key = ModifierKeys.None;
                    }

                    SetModifier(hotkey, temp_key);
                    SetKey(hotkey);
                }
                else if (split_string.Length > 2)
                {
                    List<ModifierKeys> temp = new List<ModifierKeys>();
                    for (int i = 0; i < split_string.Length; i++)
                    {
                        string cmp = split_string[i].Trim().ToLower();
                        if (cmp.Contains("alt"))
                        {
                            temp.Add(ModifierKeys.Alt);
                        }
                        else if (cmp.Contains("ctrl"))
                        {
                            temp.Add(ModifierKeys.Control);
                        }
                        else if (cmp.Contains("shift"))
                        {
                            temp.Add(ModifierKeys.Shift);
                        }
                        else if (cmp.Contains("win"))
                        {
                            temp.Add(ModifierKeys.Windows);
                        }
                    }

                    SetModifier(hotkey, temp);
                    SetKey(hotkey);
                }
                else
                {
                    SetModifier(hotkey, ModifierKeys.None);
                    SetKey(hotkey);
                }
            }
        }
        /// <summary> 
        /// <para>
        /// Sets the field of the given parameter.
        /// </para>
        /// </summary>
        /// <param name="hotkey">the new hotkey.</param>
        /// <param name="mod">the new modifier.</param>
        private static void SetModifier(string hotkey, ModifierKeys mod)
        {
            string typeOfHotKey = GetHotkeyType(hotkey);
            if (typeOfHotKey == "") return;

            if (typeOfHotKey == "refresh")
            {
                refreshModifier = mod;
            }
            else if (typeOfHotKey == "toggle")
            {
                toggleModifier = mod;
            }
            else if (typeOfHotKey == "stashtab")
            {
                stashTabModifier = mod;
            }
        }
        /// <summary> 
        /// List containing modifiers version of 
        /// <c>
        /// SetModifier
        /// </c>
        /// <para>
        /// Sets the field of the given parameter. 
        /// </para>
        /// </summary>
        /// <param name="hotkey">the new hotkey.</param>
        /// <param name="mods">the new modifiers.</param>
        private static void SetModifier(string hotkey, List<ModifierKeys> mods)
        {
            string typeOfHotKey = GetHotkeyType(hotkey);
            if (typeOfHotKey == "") return;

            if (typeOfHotKey == "refresh")
            {
                refreshModifiers = mods;
            }
            else if (typeOfHotKey == "toggle")
            {
                toggleModifiers = mods;
            }
            else if (typeOfHotKey == "stashtab")
            {
                stashTabModifiers = mods;
            }
            RequiresModifierKey = true;
        }
        /// <summary>
        /// Checks what type the current hotkey is and returns a string.
        /// </summary>
        /// <param name="hotkey">used to get type of hotkey</param>
        /// <returns>
        /// A string corresponding to the given <paramref name="hotkey"/>,
        /// if it fails - returns an empty string.
        /// </returns>
        private static string GetHotkeyType(string hotkey)
        {
            if (hotkey.Equals(Settings.Default.HotkeyRefresh))
            {
                return "refresh";
            }
            else if (hotkey.Equals(Settings.Default.HotkeyToggle))
            {
                return "toggle";
            }
            else if (hotkey.Equals(Settings.Default.HotkeyStashTab))
            {
                return "stashtab";
            }
            return "";
        }
        /// <summary>
        /// It will set the hotkey to be used for the corresponding type of hotkey.
        /// </summary>
        /// <param name="hotkey">used to get type of hotkey and extract key from</param>
        private static void SetKey(string hotkey)
        {
            string[] split_string = hotkey.Split('+');
            string last_element = split_string[split_string.Length - 1].Trim();
            string typeOfHotKey = GetHotkeyType(hotkey);

            if (typeOfHotKey == "refresh")
            {
                Enum.TryParse(last_element, out refreshKey);
            }
            else if (typeOfHotKey == "toggle")
            {
                Enum.TryParse(last_element, out toggleKey);
            }
            else if (typeOfHotKey == "stashtab")
            {
                Enum.TryParse(last_element, out stashTabKey);
            }
        }

        public static void RemoveToggleHotkey()
        {
            if(toggleModifier != ModifierKeys.None)
            {
                RemoveHotkey(toggleModifier, toggleKey);
            }
            else if(toggleModifiers.Count > 1)
            {
                RemoveHotkey(toggleModifiers, toggleKey);
            }
        }

        public static void RemoveStashTabHotkey()
        {
            RemoveHotkey(stashTabModifier, stashTabKey);
        }

        public static void RemoveRefreshHotkey()
        {
            RemoveHotkey(refreshModifier, refreshKey);
        }

        // Callbacks

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        //public static void RemoveReloadFilterHotkey()
        //{
        //    HotkeysManager.RemoveHotkey(reloadFilterModifier, reloadFilterKey);
        //}

        #region Native Methods

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}