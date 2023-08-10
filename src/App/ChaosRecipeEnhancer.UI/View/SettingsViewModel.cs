using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.View;

internal class SettingsViewModel : ViewModelBase
{
    public string Version => AppConstants.VersionText;
}