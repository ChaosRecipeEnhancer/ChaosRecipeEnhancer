using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

// RingFilterStyleSettings
public partial class UserSettings : IUserSettings
{
    public int LootFilterStylesRingTextFontSize
    {
        get => Settings.Default.LootFilterStylesRingTextFontSize;
        set
        {
            if (Settings.Default.LootFilterStylesRingTextFontSize != value)
            {
                Settings.Default.LootFilterStylesRingTextFontSize = value;
                Save();
            }
        }
    }

    public string LootFilterStylesRingTextColor
    {
        get => Settings.Default.LootFilterStylesRingTextColor;
        set
        {
            if (Settings.Default.LootFilterStylesRingTextColor != value)
            {
                Settings.Default.LootFilterStylesRingTextColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesRingBorderColor
    {
        get => Settings.Default.LootFilterStylesRingBorderColor;
        set
        {
            if (Settings.Default.LootFilterStylesRingBorderColor != value)
            {
                Settings.Default.LootFilterStylesRingBorderColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesRingBackgroundColor
    {
        get => Settings.Default.LootFilterStylesRingBackgroundColor;
        set
        {
            if (Settings.Default.LootFilterStylesRingBackgroundColor != value)
            {
                Settings.Default.LootFilterStylesRingBackgroundColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesRingAlwaysActive
    {
        get => Settings.Default.LootFilterStylesRingAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterStylesRingAlwaysActive != value)
            {
                Settings.Default.LootFilterStylesRingAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesRingAlwaysDisabled
    {
        get => Settings.Default.LootFilterStylesRingAlwaysDisabled;
        set
        {
            if (Settings.Default.LootFilterStylesRingAlwaysDisabled != value)
            {
                Settings.Default.LootFilterStylesRingAlwaysDisabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesRingMapIconEnabled
    {
        get => Settings.Default.LootFilterStylesRingMapIconEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesRingMapIconEnabled != value)
            {
                Settings.Default.LootFilterStylesRingMapIconEnabled = value;
                Save();
            }
        }
    }

    public int LootFilterStylesRingMapIconColor
    {
        get => Settings.Default.LootFilterStylesRingMapIconColor;
        set
        {
            if (Settings.Default.LootFilterStylesRingMapIconColor != value)
            {
                Settings.Default.LootFilterStylesRingMapIconColor = value;
                Save();
            }
        }
    }

    public int LootFilterStylesRingMapIconSize
    {
        get => Settings.Default.LootFilterStylesRingMapIconSize;
        set
        {
            if (Settings.Default.LootFilterStylesRingMapIconSize != value)
            {
                Settings.Default.LootFilterStylesRingMapIconSize = value;
                Save();
            }
        }
    }

    public int LootFilterStylesRingMapIconShape
    {
        get => Settings.Default.LootFilterStylesRingMapIconShape;
        set
        {
            if (Settings.Default.LootFilterStylesRingMapIconShape != value)
            {
                Settings.Default.LootFilterStylesRingMapIconShape = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesRingBeamEnabled
    {
        get => Settings.Default.LootFilterStylesRingBeamEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesRingBeamEnabled != value)
            {
                Settings.Default.LootFilterStylesRingBeamEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesRingBeamTemporary
    {
        get => Settings.Default.LootFilterStylesRingBeamTemporary;
        set
        {
            if (Settings.Default.LootFilterStylesRingBeamTemporary != value)
            {
                Settings.Default.LootFilterStylesRingBeamTemporary = value;
                Save();
            }
        }
    }

    public int LootFilterStylesRingBeamColor
    {
        get => Settings.Default.LootFilterStylesRingBeamColor;
        set
        {
            if (Settings.Default.LootFilterStylesRingBeamColor != value)
            {
                Settings.Default.LootFilterStylesRingBeamColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesRingTextColorEnabled
    {
        get => Settings.Default.LootFilterStylesRingTextColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesRingTextColorEnabled != value)
            {
                Settings.Default.LootFilterStylesRingTextColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesRingBorderColorEnabled
    {
        get => Settings.Default.LootFilterStylesRingBorderColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesRingBorderColorEnabled != value)
            {
                Settings.Default.LootFilterStylesRingBorderColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesRingBackgroundColorEnabled
    {
        get => Settings.Default.LootFilterStylesRingBackgroundColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesRingBackgroundColorEnabled != value)
            {
                Settings.Default.LootFilterStylesRingBackgroundColorEnabled = value;
                Save();
            }
        }
    }
}
