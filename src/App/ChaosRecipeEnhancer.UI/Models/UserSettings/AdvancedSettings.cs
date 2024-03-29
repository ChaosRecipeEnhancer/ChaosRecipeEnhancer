using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Advanced Settings.
/// </summary>
public partial class UserSettings : IUserSettings
{
    public bool DoNotPreserveLowItemLevelGear
    {
        get => Settings.Default.DoNotPreserveLowItemLevelGear;
        set
        {
            if (Settings.Default.DoNotPreserveLowItemLevelGear != value)
            {
                Settings.Default.DoNotPreserveLowItemLevelGear = value;
                Save();
            }
        }
    }
}