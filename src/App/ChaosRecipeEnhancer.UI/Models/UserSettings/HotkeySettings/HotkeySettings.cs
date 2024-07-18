using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to Hotkeys.
/// </summary>
public partial class UserSettings : IUserSettings
{
    public string FetchStashHotkey
    {
        get => Settings.Default.FetchStashHotkey;
        set
        {
            if (Settings.Default.FetchStashHotkey != value)
            {
                Settings.Default.FetchStashHotkey = value;
                Save();
            }
        }
    }

    public string ToggleSetTrackerOverlayHotkey
    {
        get => Settings.Default.ToggleSetTrackerOverlayHotkey;
        set
        {
            if (Settings.Default.ToggleSetTrackerOverlayHotkey != value)
            {
                Settings.Default.ToggleSetTrackerOverlayHotkey = value;
                Save();
            }
        }
    }

    public string ToggleStashTabOverlayHotkey
    {
        get => Settings.Default.ToggleStashTabOverlayHotkey;
        set
        {
            if (Settings.Default.ToggleStashTabOverlayHotkey != value)
            {
                Settings.Default.ToggleStashTabOverlayHotkey = value;
                Save();
            }
        }
    }

    public string ReloadFilterHotkey
    {
        get => Settings.Default.ReloadFilterHotkey;
        set
        {
            if (Settings.Default.ReloadFilterHotkey != value)
            {
                Settings.Default.ReloadFilterHotkey = value;
                Save();
            }
        }
    }
}