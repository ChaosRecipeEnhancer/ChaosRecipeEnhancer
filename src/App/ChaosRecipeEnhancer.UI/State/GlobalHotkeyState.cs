using ChaosRecipeEnhancer.UI.Models.Hotkeys;
using ChaosRecipeEnhancer.UI.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.Constants;

namespace ChaosRecipeEnhancer.UI.State;

public static class GlobalHotkeyState
{
    public static Dictionary<HotkeyTypes, GlobalHotkey> Hotkeys { get; } = new();

    private static void AddHotkey(HotkeyTypes hotkeyType, ModifierKeys modifier, Key key, Action callbackMethod, TimeSpan cooldown)
    {
        Hotkeys.Add(hotkeyType, new GlobalHotkey(modifier, key, callbackMethod, cooldown, new DateTime()));
    }

    public static void SetRefreshHotkeyFromSettings(Action callback)
        => SetHotkey(HotkeyTypes.FetchStashData, callback);

    public static void SetToggleHotkeyFromSettings(Action callback)
        => SetHotkey(HotkeyTypes.ToggleSetTrackerOverlay, callback);

    public static void SetStashTabHotkeyFromSettings(Action callback)
        => SetHotkey(HotkeyTypes.ToggleStashTabOverlay, callback);

    public static void SetReloadFilterHotkeyFromSettings(Action callback)
        => SetHotkey(HotkeyTypes.ReloadItemFilter, callback);

    private static void SetHotkey(HotkeyTypes hotkeyType, Action callback)
    {
        string settingKey = hotkeyType switch
        {
            HotkeyTypes.FetchStashData => Settings.Default.FetchStashHotkey,
            HotkeyTypes.ToggleSetTrackerOverlay => Settings.Default.ToggleSetTrackerOverlayHotkey,
            HotkeyTypes.ToggleStashTabOverlay => Settings.Default.ToggleStashTabOverlayHotkey,
            HotkeyTypes.ReloadItemFilter => Settings.Default.ReloadFilterHotkey,
            _ => throw new ArgumentException("Hotkey type not supported")
        };

        TimeSpan cooldown = hotkeyType switch
        {
            HotkeyTypes.FetchStashData => HotkeyDefaults.FetchStashDataHotkeyCooldown,
            HotkeyTypes.ToggleSetTrackerOverlay => HotkeyDefaults.ToggleSetTrackerHotkeyCooldown,
            HotkeyTypes.ToggleStashTabOverlay => HotkeyDefaults.ToggleStashTabHotkeyCooldown,
            HotkeyTypes.ReloadItemFilter => HotkeyDefaults.ReloadFilterHotkeyCooldown,
            _ => throw new ArgumentException("Hotkey type not supported")
        };

        if (settingKey != "< not set >")
        {
            var hotkeyParts = settingKey.Split('+');
            ModifierKeys modifier = ModifierKeys.None;
            Key key;

            if (hotkeyParts.Length > 1)
            {
                modifier = ParseModifier(hotkeyParts[0].Trim());
                Enum.TryParse(hotkeyParts[1].Trim(), out key);
            }
            else
            {
                Enum.TryParse(hotkeyParts[0].Trim(), out key);
            }

            // Here you can call AddHotkey with the parsed modifier and key
            AddHotkey(hotkeyType, modifier, key, callback, cooldown);
        }
    }

    private static void RemoveHotkey(HotkeyTypes hotkeyType)
    {
        Hotkeys.Remove(hotkeyType);
    }

    public static void RemoveAllHotkeys()
    {
        RemoveFetchStashDataHotkey();
        RemoveToggleStashTabOverlayHotkey();
        RemoveToggleSetTrackerOverlayHotkey();
        RemoveReloadItemFilterHotkey();
    }

    public static void RemoveToggleSetTrackerOverlayHotkey()
        => RemoveHotkey(HotkeyTypes.ToggleSetTrackerOverlay);

    public static void RemoveToggleStashTabOverlayHotkey()
        => RemoveHotkey(HotkeyTypes.ToggleStashTabOverlay);

    public static void RemoveFetchStashDataHotkey()
        => RemoveHotkey(HotkeyTypes.FetchStashData);

    public static void RemoveReloadItemFilterHotkey()
        => RemoveHotkey(HotkeyTypes.ReloadItemFilter);

    private static ModifierKeys ParseModifier(string modifierString)
    {
        return modifierString switch
        {
            "Ctrl" => ModifierKeys.Control,
            "Alt" => ModifierKeys.Alt,
            "Win" => ModifierKeys.Windows,
            "Shift" => ModifierKeys.Shift,
            _ => ModifierKeys.None,
        };
    }
}