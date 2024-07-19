using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

// WeaponFilterStyleSettings
public partial class UserSettings : IUserSettings
{
    // Old Settings
    public string LootFilterWeaponColor
    {
        get => Settings.Default.LootFilterWeaponColor;
        set
        {
            if (Settings.Default.LootFilterWeaponColor != value)
            {
                Settings.Default.LootFilterWeaponColor = value;
                Save();
            }
        }
    }

    public bool LootFilterWeaponsAlwaysActive
    {
        get => Settings.Default.LootFilterWeaponsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterWeaponsAlwaysActive != value)
            {
                Settings.Default.LootFilterWeaponsAlwaysActive = value;
                Save();
            }
        }
    }

    // New Settings
    public int LootFilterStylesWeaponTextFontSize
    {
        get => Settings.Default.LootFilterStylesWeaponTextFontSize;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponTextFontSize != value)
            {
                Settings.Default.LootFilterStylesWeaponTextFontSize = value;
                Save();
            }
        }
    }

    public string LootFilterStylesWeaponTextColor
    {
        get => Settings.Default.LootFilterStylesWeaponTextColor;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponTextColor != value)
            {
                Settings.Default.LootFilterStylesWeaponTextColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesWeaponBorderColor
    {
        get => Settings.Default.LootFilterStylesWeaponBorderColor;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponBorderColor != value)
            {
                Settings.Default.LootFilterStylesWeaponBorderColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesWeaponBackgroundColor
    {
        get => Settings.Default.LootFilterStylesWeaponBackgroundColor;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponBackgroundColor != value)
            {
                Settings.Default.LootFilterStylesWeaponBackgroundColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesWeaponAlwaysActive
    {
        get => Settings.Default.LootFilterStylesWeaponAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponAlwaysActive != value)
            {
                Settings.Default.LootFilterStylesWeaponAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesWeaponAlwaysDisabled
    {
        get => Settings.Default.LootFilterStylesWeaponAlwaysDisabled;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponAlwaysDisabled != value)
            {
                Settings.Default.LootFilterStylesWeaponAlwaysDisabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesWeaponMapIconEnabled
    {
        get => Settings.Default.LootFilterStylesWeaponMapIconEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponMapIconEnabled != value)
            {
                Settings.Default.LootFilterStylesWeaponMapIconEnabled = value;
                Save();
            }
        }
    }

    public int LootFilterStylesWeaponMapIconColor
    {
        get => Settings.Default.LootFilterStylesWeaponMapIconColor;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponMapIconColor != value)
            {
                Settings.Default.LootFilterStylesWeaponMapIconColor = value;
                Save();
            }
        }
    }

    public int LootFilterStylesWeaponMapIconSize
    {
        get => Settings.Default.LootFilterStylesWeaponMapIconSize;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponMapIconSize != value)
            {
                Settings.Default.LootFilterStylesWeaponMapIconSize = value;
                Save();
            }
        }
    }

    public int LootFilterStylesWeaponMapIconShape
    {
        get => Settings.Default.LootFilterStylesWeaponMapIconShape;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponMapIconShape != value)
            {
                Settings.Default.LootFilterStylesWeaponMapIconShape = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesWeaponBeamEnabled
    {
        get => Settings.Default.LootFilterStylesWeaponBeamEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponBeamEnabled != value)
            {
                Settings.Default.LootFilterStylesWeaponBeamEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesWeaponBeamTemporary
    {
        get => Settings.Default.LootFilterStylesWeaponBeamTemporary;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponBeamTemporary != value)
            {
                Settings.Default.LootFilterStylesWeaponBeamTemporary = value;
                Save();
            }
        }
    }

    public int LootFilterStylesWeaponBeamColor
    {
        get => Settings.Default.LootFilterStylesWeaponBeamColor;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponBeamColor != value)
            {
                Settings.Default.LootFilterStylesWeaponBeamColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesWeaponTextColorEnabled
    {
        get => Settings.Default.LootFilterStylesWeaponTextColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponTextColorEnabled != value)
            {
                Settings.Default.LootFilterStylesWeaponTextColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesWeaponBorderColorEnabled
    {
        get => Settings.Default.LootFilterStylesWeaponBorderColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponBorderColorEnabled != value)
            {
                Settings.Default.LootFilterStylesWeaponBorderColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesWeaponBackgroundColorEnabled
    {
        get => Settings.Default.LootFilterStylesWeaponBackgroundColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesWeaponBackgroundColorEnabled != value)
            {
                Settings.Default.LootFilterStylesWeaponBackgroundColorEnabled = value;
                Save();
            }
        }
    }
}
