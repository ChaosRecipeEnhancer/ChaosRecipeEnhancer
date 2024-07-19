using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Color Settings.
/// </summary>
public partial class UserSettings : IUserSettings
{
    public string LootFilterBeltColor
    {
        get => Settings.Default.LootFilterBeltColor;
        set
        {
            if (Settings.Default.LootFilterBeltColor != value)
            {
                Settings.Default.LootFilterBeltColor = value;
                Save();
            }
        }
    }

    public string LootFilterBootsColor
    {
        get => Settings.Default.LootFilterBootsColor;
        set
        {
            if (Settings.Default.LootFilterBootsColor != value)
            {
                Settings.Default.LootFilterBootsColor = value;
                Save();
            }
        }
    }

    public string LootFilterGlovesColor
    {
        get => Settings.Default.LootFilterGlovesColor;
        set
        {
            if (Settings.Default.LootFilterGlovesColor != value)
            {
                Settings.Default.LootFilterGlovesColor = value;
                Save();
            }
        }
    }

    public string LootFilterHelmetColor
    {
        get => Settings.Default.LootFilterHelmetColor;
        set
        {
            if (Settings.Default.LootFilterHelmetColor != value)
            {
                Settings.Default.LootFilterHelmetColor = value;
                Save();
            }
        }
    }

    public string LootFilterBodyArmourColor
    {
        get => Settings.Default.LootFilterBodyArmourColor;
        set
        {
            if (Settings.Default.LootFilterBodyArmourColor != value)
            {
                Settings.Default.LootFilterBodyArmourColor = value;
                Save();
            }
        }
    }

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
}