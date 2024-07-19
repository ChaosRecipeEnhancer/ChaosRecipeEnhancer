using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

// HelmetFilterStyleSettings
public partial class UserSettings : IUserSettings
{
    public int LootFilterStylesHelmetTextFontSize
    {
        get => Settings.Default.LootFilterStylesHelmetTextFontSize;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetTextFontSize != value)
            {
                Settings.Default.LootFilterStylesHelmetTextFontSize = value;
                Save();
            }
        }
    }

    public string LootFilterStylesHelmetTextColor
    {
        get => Settings.Default.LootFilterStylesHelmetTextColor;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetTextColor != value)
            {
                Settings.Default.LootFilterStylesHelmetTextColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesHelmetBorderColor
    {
        get => Settings.Default.LootFilterStylesHelmetBorderColor;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetBorderColor != value)
            {
                Settings.Default.LootFilterStylesHelmetBorderColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesHelmetBackgroundColor
    {
        get => Settings.Default.LootFilterStylesHelmetBackgroundColor;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetBackgroundColor != value)
            {
                Settings.Default.LootFilterStylesHelmetBackgroundColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesHelmetAlwaysActive
    {
        get => Settings.Default.LootFilterStylesHelmetAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetAlwaysActive != value)
            {
                Settings.Default.LootFilterStylesHelmetAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesHelmetAlwaysDisabled
    {
        get => Settings.Default.LootFilterStylesHelmetAlwaysDisabled;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetAlwaysDisabled != value)
            {
                Settings.Default.LootFilterStylesHelmetAlwaysDisabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesHelmetMapIconEnabled
    {
        get => Settings.Default.LootFilterStylesHelmetMapIconEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetMapIconEnabled != value)
            {
                Settings.Default.LootFilterStylesHelmetMapIconEnabled = value;
                Save();
            }
        }
    }

    public int LootFilterStylesHelmetMapIconColor
    {
        get => Settings.Default.LootFilterStylesHelmetMapIconColor;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetMapIconColor != value)
            {
                Settings.Default.LootFilterStylesHelmetMapIconColor = value;
                Save();
            }
        }
    }

    public int LootFilterStylesHelmetMapIconSize
    {
        get => Settings.Default.LootFilterStylesHelmetMapIconSize;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetMapIconSize != value)
            {
                Settings.Default.LootFilterStylesHelmetMapIconSize = value;
                Save();
            }
        }
    }

    public int LootFilterStylesHelmetMapIconShape
    {
        get => Settings.Default.LootFilterStylesHelmetMapIconShape;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetMapIconShape != value)
            {
                Settings.Default.LootFilterStylesHelmetMapIconShape = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesHelmetBeamEnabled
    {
        get => Settings.Default.LootFilterStylesHelmetBeamEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetBeamEnabled != value)
            {
                Settings.Default.LootFilterStylesHelmetBeamEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesHelmetBeamTemporary
    {
        get => Settings.Default.LootFilterStylesHelmetBeamTemporary;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetBeamTemporary != value)
            {
                Settings.Default.LootFilterStylesHelmetBeamTemporary = value;
                Save();
            }
        }
    }

    public int LootFilterStylesHelmetBeamColor
    {
        get => Settings.Default.LootFilterStylesHelmetBeamColor;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetBeamColor != value)
            {
                Settings.Default.LootFilterStylesHelmetBeamColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesHelmetTextColorEnabled
    {
        get => Settings.Default.LootFilterStylesHelmetTextColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetTextColorEnabled != value)
            {
                Settings.Default.LootFilterStylesHelmetTextColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesHelmetBorderColorEnabled
    {
        get => Settings.Default.LootFilterStylesHelmetBorderColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetBorderColorEnabled != value)
            {
                Settings.Default.LootFilterStylesHelmetBorderColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesHelmetBackgroundColorEnabled
    {
        get => Settings.Default.LootFilterStylesHelmetBackgroundColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesHelmetBackgroundColorEnabled != value)
            {
                Settings.Default.LootFilterStylesHelmetBackgroundColorEnabled = value;
                Save();
            }
        }
    }
}
