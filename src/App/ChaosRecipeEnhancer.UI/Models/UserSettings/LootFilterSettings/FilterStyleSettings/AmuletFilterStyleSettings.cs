using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

// AmuletFilterStyleSettings
public partial class UserSettings : IUserSettings
{
    // Old Settings
    public string LootFilterAmuletColor
    {
        get => Settings.Default.LootFilterAmuletColor;
        set
        {
            if (Settings.Default.LootFilterAmuletColor != value)
            {
                Settings.Default.LootFilterAmuletColor = value;
                Save();
            }
        }
    }

    public bool LootFilterAmuletsAlwaysActive
    {
        get => Settings.Default.LootFilterAmuletsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterAmuletsAlwaysActive != value)
            {
                Settings.Default.LootFilterAmuletsAlwaysActive = value;
                Save();
            }
        }
    }

    // New Settings
    public int LootFilterStylesAmuletTextFontSize
    {
        get => Settings.Default.LootFilterStylesAmuletTextFontSize;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletTextFontSize != value)
            {
                Settings.Default.LootFilterStylesAmuletTextFontSize = value;
                Save();
            }
        }
    }

    public string LootFilterStylesAmuletTextColor
    {
        get => Settings.Default.LootFilterStylesAmuletTextColor;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletTextColor != value)
            {
                Settings.Default.LootFilterStylesAmuletTextColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesAmuletBorderColor
    {
        get => Settings.Default.LootFilterStylesAmuletBorderColor;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletBorderColor != value)
            {
                Settings.Default.LootFilterStylesAmuletBorderColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesAmuletBackgroundColor
    {
        get => Settings.Default.LootFilterStylesAmuletBackgroundColor;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletBackgroundColor != value)
            {
                Settings.Default.LootFilterStylesAmuletBackgroundColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesAmuletAlwaysActive
    {
        get => Settings.Default.LootFilterStylesAmuletAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletAlwaysActive != value)
            {
                Settings.Default.LootFilterStylesAmuletAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesAmuletAlwaysDisabled
    {
        get => Settings.Default.LootFilterStylesAmuletAlwaysDisabled;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletAlwaysDisabled != value)
            {
                Settings.Default.LootFilterStylesAmuletAlwaysDisabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesAmuletMapIconEnabled
    {
        get => Settings.Default.LootFilterStylesAmuletMapIconEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletMapIconEnabled != value)
            {
                Settings.Default.LootFilterStylesAmuletMapIconEnabled = value;
                Save();
            }
        }
    }

    public int LootFilterStylesAmuletMapIconColor
    {
        get => Settings.Default.LootFilterStylesAmuletMapIconColor;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletMapIconColor != value)
            {
                Settings.Default.LootFilterStylesAmuletMapIconColor = value;
                Save();
            }
        }
    }

    public int LootFilterStylesAmuletMapIconSize
    {
        get => Settings.Default.LootFilterStylesAmuletMapIconSize;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletMapIconSize != value)
            {
                Settings.Default.LootFilterStylesAmuletMapIconSize = value;
                Save();
            }
        }
    }

    public int LootFilterStylesAmuletMapIconShape
    {
        get => Settings.Default.LootFilterStylesAmuletMapIconShape;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletMapIconShape != value)
            {
                Settings.Default.LootFilterStylesAmuletMapIconShape = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesAmuletBeamEnabled
    {
        get => Settings.Default.LootFilterStylesAmuletBeamEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletBeamEnabled != value)
            {
                Settings.Default.LootFilterStylesAmuletBeamEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesAmuletBeamTemporary
    {
        get => Settings.Default.LootFilterStylesAmuletBeamTemporary;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletBeamTemporary != value)
            {
                Settings.Default.LootFilterStylesAmuletBeamTemporary = value;
                Save();
            }
        }
    }

    public int LootFilterStylesAmuletBeamColor
    {
        get => Settings.Default.LootFilterStylesAmuletBeamColor;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletBeamColor != value)
            {
                Settings.Default.LootFilterStylesAmuletBeamColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesAmuletTextColorEnabled
    {
        get => Settings.Default.LootFilterStylesAmuletTextColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletTextColorEnabled != value)
            {
                Settings.Default.LootFilterStylesAmuletTextColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesAmuletBorderColorEnabled
    {
        get => Settings.Default.LootFilterStylesAmuletBorderColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletBorderColorEnabled != value)
            {
                Settings.Default.LootFilterStylesAmuletBorderColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesAmuletBackgroundColorEnabled
    {
        get => Settings.Default.LootFilterStylesAmuletBackgroundColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesAmuletBackgroundColorEnabled != value)
            {
                Settings.Default.LootFilterStylesAmuletBackgroundColorEnabled = value;
                Save();
            }
        }
    }
}
