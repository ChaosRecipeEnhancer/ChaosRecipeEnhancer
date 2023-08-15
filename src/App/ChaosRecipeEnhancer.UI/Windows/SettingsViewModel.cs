using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;

namespace ChaosRecipeEnhancer.UI.Windows;

internal class SettingsViewModel : ViewModelBase
{
    public string Version => AppInfo.VersionText;
}