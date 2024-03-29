using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Loot Filter Settings.
/// </summary>
public partial class UserSettings : IUserSettings
{
    public bool LootFilterManipulationEnabled
    {
        get => Settings.Default.LootFilterManipulationEnabled;
        set
        {
            if (Settings.Default.LootFilterManipulationEnabled != value)
            {
                Settings.Default.LootFilterManipulationEnabled = value;
                Save();
            }
        }
    }

    public string LootFilterFileLocation
    {
        get => Settings.Default.LootFilterFileLocation;
        set
        {
            if (Settings.Default.LootFilterFileLocation != value)
            {
                Settings.Default.LootFilterFileLocation = value;
                Save();
            }
        }
    }

    public bool LootFilterIconsEnabled
    {
        get => Settings.Default.LootFilterIconsEnabled;
        set
        {
            if (Settings.Default.LootFilterIconsEnabled != value)
            {
                Settings.Default.LootFilterIconsEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterRingsAlwaysActive
    {
        get => Settings.Default.LootFilterRingsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterRingsAlwaysActive != value)
            {
                Settings.Default.LootFilterRingsAlwaysActive = value;
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

    public bool LootFilterBeltsAlwaysActive
    {
        get => Settings.Default.LootFilterBeltsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterBeltsAlwaysActive != value)
            {
                Settings.Default.LootFilterBeltsAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterGlovesAlwaysActive
    {
        get => Settings.Default.LootFilterGlovesAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterGlovesAlwaysActive != value)
            {
                Settings.Default.LootFilterGlovesAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterBootsAlwaysActive
    {
        get => Settings.Default.LootFilterBootsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterBootsAlwaysActive != value)
            {
                Settings.Default.LootFilterBootsAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterHelmetsAlwaysActive
    {
        get => Settings.Default.LootFilterHelmetsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterHelmetsAlwaysActive != value)
            {
                Settings.Default.LootFilterHelmetsAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterBodyArmourAlwaysActive
    {
        get => Settings.Default.LootFilterBodyArmourAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterBodyArmourAlwaysActive != value)
            {
                Settings.Default.LootFilterBodyArmourAlwaysActive = value;
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
}
