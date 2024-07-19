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

    public bool LootFilterSpaceSavingHideLargeWeapons
    {
        get => Settings.Default.LootFilterSpaceSavingHideLargeWeapons;
        set
        {
            if (Settings.Default.LootFilterSpaceSavingHideLargeWeapons != value)
            {
                Settings.Default.LootFilterSpaceSavingHideLargeWeapons = value;
                Save();
            }
        }
    }

    public bool LootFilterSpaceSavingHideOffHand
    {
        get => Settings.Default.LootFilterSpaceSavingHideOffHand;
        set
        {
            if (Settings.Default.LootFilterSpaceSavingHideOffHand != value)
            {
                Settings.Default.LootFilterSpaceSavingHideOffHand = value;
                Save();
            }
        }
    }
}
