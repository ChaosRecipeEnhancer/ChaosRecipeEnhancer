using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Set Tracker Overlay.
/// </summary>
public partial class UserSettings : IUserSettings
{
    public double SetTrackerOverlayTopPosition
    {
        get => Settings.Default.SetTrackerOverlayTopPosition;
        set
        {
            if (Settings.Default.SetTrackerOverlayTopPosition != value)
            {
                Settings.Default.SetTrackerOverlayTopPosition = value;
                Save();
            }
        }
    }

    public double SetTrackerOverlayLeftPosition
    {
        get => Settings.Default.SetTrackerOverlayLeftPosition;
        set
        {
            if (Settings.Default.SetTrackerOverlayLeftPosition != value)
            {
                Settings.Default.SetTrackerOverlayLeftPosition = value;
                Save();
            }
        }
    }

    public SetTrackerOverlayItemCounterDisplayMode SetTrackerOverlayItemCounterDisplayMode
    {
        get => (SetTrackerOverlayItemCounterDisplayMode)Settings.Default.SetTrackerOverlayItemCounterDisplayMode;
        set
        {
            if (Settings.Default.SetTrackerOverlayItemCounterDisplayMode != (int)value)
            {
                Settings.Default.SetTrackerOverlayItemCounterDisplayMode = (int)value;
                Save();
            }
        }
    }

    public SetTrackerOverlayMode SetTrackerOverlayMode
    {
        get => (SetTrackerOverlayMode)Settings.Default.SetTrackerOverlayDisplayMode;
        set
        {
            if (Settings.Default.SetTrackerOverlayDisplayMode != (int)value)
            {
                Settings.Default.SetTrackerOverlayDisplayMode = (int)value;
                Save();
            }
        }
    }

    public bool SetTrackerOverlayOverlayLockPositionEnabled
    {
        get => Settings.Default.SetTrackerOverlayOverlayLockPositionEnabled;
        set
        {
            if (Settings.Default.SetTrackerOverlayOverlayLockPositionEnabled != value)
            {
                Settings.Default.SetTrackerOverlayOverlayLockPositionEnabled = value;
                Save();
            }
        }
    }

    public bool SilenceNeedItemsMessage
    {
        get => Settings.Default.SilenceNeedItemsMessage;
        set
        {
            if (Settings.Default.SilenceNeedItemsMessage != value)
            {
                Settings.Default.SilenceNeedItemsMessage = value;
                Save();
            }
        }
    }

    public bool SilenceSetsFullMessage
    {
        get => Settings.Default.SilenceSetsFullMessage;
        set
        {
            if (Settings.Default.SilenceSetsFullMessage != value)
            {
                Settings.Default.SilenceSetsFullMessage = value;
                Save();
            }
        }
    }
}
