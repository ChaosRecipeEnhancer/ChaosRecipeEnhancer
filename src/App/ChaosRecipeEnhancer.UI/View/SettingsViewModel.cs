using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.View;

internal class SettingsViewModel : ViewModelBase
{
    public Settings Settings { get; } = Settings.Default;
}