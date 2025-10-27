using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Stash Tab Overlay.
/// </summary>
public partial class UserSettings : IUserSettings
{
    public float StashTabOverlayOpacity
    {
        get => Settings.Default.StashTabOverlayOpacity;
        set
        {
            if (Settings.Default.StashTabOverlayOpacity != value)
            {
                Settings.Default.StashTabOverlayOpacity = value;
                Save();
            }
        }
    }

    public string StashTabOverlayBackgroundColor
    {
        get => Settings.Default.StashTabOverlayBackgroundColor;
        set
        {
            if (Settings.Default.StashTabOverlayBackgroundColor != value)
            {
                Settings.Default.StashTabOverlayBackgroundColor = value;
                Save();
            }
        }
    }

    public string StashTabOverlayHighlightColor
    {
        get => Settings.Default.StashTabOverlayHighlightColor;
        set
        {
            if (Settings.Default.StashTabOverlayHighlightColor != value)
            {
                Settings.Default.StashTabOverlayHighlightColor = value;
                Save();
            }
        }
    }

    public int StashTabOverlayHighlightMode
    {
        get => Settings.Default.StashTabOverlayHighlightMode;
        set
        {
            if (Settings.Default.StashTabOverlayHighlightMode != value)
            {
                Settings.Default.StashTabOverlayHighlightMode = value;
                Save();
            }
        }
    }

    public double StashTabOverlayIndividualTabMargin
    {
        get => Settings.Default.StashTabOverlayIndividualTabMargin;
        set
        {
            if (Settings.Default.StashTabOverlayIndividualTabMargin != value)
            {
                Settings.Default.StashTabOverlayIndividualTabMargin = value;
                Save();
            }
        }
    }

    public double StashTabOverlayIndividualTabHeaderPadding
    {
        get => Settings.Default.StashTabOverlayIndividualTabHeaderPadding;
        set
        {
            if (Settings.Default.StashTabOverlayIndividualTabHeaderPadding != value)
            {
                Settings.Default.StashTabOverlayIndividualTabHeaderPadding = value;
                Save();
            }
        }
    }

    public double StashTabOverlayIndividualTabHeaderGap
    {
        get => Settings.Default.StashTabOverlayIndividualTabHeaderGap;
        set
        {
            if (Settings.Default.StashTabOverlayIndividualTabHeaderGap != value)
            {
                Settings.Default.StashTabOverlayIndividualTabHeaderGap = value;
                Save();
            }
        }
    }

    public double StashTabOverlayTopPosition
    {
        get => Settings.Default.StashTabOverlayTopPosition;
        set
        {
            if (Settings.Default.StashTabOverlayTopPosition != value)
            {
                Settings.Default.StashTabOverlayTopPosition = value;
                Save();
            }
        }
    }

    public double StashTabOverlayLeftPosition
    {
        get => Settings.Default.StashTabOverlayLeftPosition;
        set
        {
            if (Settings.Default.StashTabOverlayLeftPosition != value)
            {
                Settings.Default.StashTabOverlayLeftPosition = value;
                Save();
            }
        }
    }

    public int StashTabOverlaySessionScreen
    {
        get => Settings.Default.StashTabOverlaySessionScreen;
        set
        {
            if (Settings.Default.StashTabOverlaySessionScreen != value)
            {
                Settings.Default.StashTabOverlaySessionScreen = value;
                Save();
            }
        }
    }

    public bool StashTabOverlayModified
    {
        get => Settings.Default.StashTabOverlayModified;
        set
        {
            if (Settings.Default.StashTabOverlayModified != value)
            {
                Settings.Default.StashTabOverlayModified = value;
                Save();
            }
        }
    }

    public double StashTabOverlayHeight
    {
        get => Settings.Default.StashTabOverlayHeight;
        set
        {
            if (Settings.Default.StashTabOverlayHeight != value)
            {
                Settings.Default.StashTabOverlayHeight = value;
                Save();
            }
        }
    }

    public double StashTabOverlayWidth
    {
        get => Settings.Default.StashTabOverlayWidth;
        set
        {
            if (Settings.Default.StashTabOverlayWidth != value)
            {
                Settings.Default.StashTabOverlayWidth = value;
                Save();
            }
        }
    }

    public string StashTabOverlayTabDefaultBackgroundColor
    {
        get => Settings.Default.StashTabOverlayTabDefaultBackgroundColor;
        set
        {
            if (Settings.Default.StashTabOverlayTabDefaultBackgroundColor != value)
            {
                Settings.Default.StashTabOverlayTabDefaultBackgroundColor = value;
                Save();
            }
        }
    }

    public double StashTabOverlayTabGroupBottomMargin
    {
        get => Settings.Default.StashTabOverlayTabGroupBottomMargin;
        set
        {
            if (Settings.Default.StashTabOverlayTabGroupBottomMargin != value)
            {
                Settings.Default.StashTabOverlayTabGroupBottomMargin = value;
                Save();
            }
        }
    }

    public float StashTabOverlayTabOpacity
    {
        get => Settings.Default.StashTabOverlayTabOpacity;
        set
        {
            if (Settings.Default.StashTabOverlayTabOpacity != value)
            {
                Settings.Default.StashTabOverlayTabOpacity = value;
                Save();
            }
        }
    }
}