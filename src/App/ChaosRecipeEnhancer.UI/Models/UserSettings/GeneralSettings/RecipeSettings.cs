using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Chaos Recipe Enhancer Recipe Settings.
/// </summary>
public partial class UserSettings : IUserSettings
{
    public bool ChaosRecipeTrackingEnabled
    {
        get => Settings.Default.ChaosRecipeTrackingEnabled;
        set
        {
            if (Settings.Default.ChaosRecipeTrackingEnabled != value)
            {
                Settings.Default.ChaosRecipeTrackingEnabled = value;
                Save();
            }
        }
    }

    public int ActiveRecipeType
    {
        get => Settings.Default.ActiveRecipeType;
        set
        {
            if (Settings.Default.ActiveRecipeType != value)
            {
                Settings.Default.ActiveRecipeType = value;
                Save();
            }
        }
    }

    public int TargetInfluenceType
    {
        get => Settings.Default.TargetInfluenceType;
        set
        {
            if (Settings.Default.TargetInfluenceType != value)
            {
                Settings.Default.TargetInfluenceType = value;
                Save();
            }
        }
    }

    public bool IncludeIdentifiedItemsEnabled
    {
        get => Settings.Default.IncludeIdentifiedItemsEnabled;
        set
        {
            if (Settings.Default.IncludeIdentifiedItemsEnabled != value)
            {
                Settings.Default.IncludeIdentifiedItemsEnabled = value;
                Save();
            }
        }
    }

    public int FullSetThreshold
    {
        get => Settings.Default.FullSetThreshold;
        set
        {
            if (Settings.Default.FullSetThreshold != value)
            {
                Settings.Default.FullSetThreshold = value;
                Save();
            }
        }
    }

    public bool VendorSetsEarly
    {
        get => Settings.Default.VendorSetsEarly;
        set
        {
            if (Settings.Default.VendorSetsEarly != value)
            {
                Settings.Default.VendorSetsEarly = value;
                Save();
            }
        }
    }
}
