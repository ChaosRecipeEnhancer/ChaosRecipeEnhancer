using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Windows;

internal class SettingsViewModel : ViewModelBase
{
    public string Version => AppInfo.VersionText;
}