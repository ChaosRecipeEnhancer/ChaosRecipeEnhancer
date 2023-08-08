using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;

internal class PathOfExileAccountFormViewModel : ViewModelBase
{
    public Settings Settings { get; } = Settings.Default;
}