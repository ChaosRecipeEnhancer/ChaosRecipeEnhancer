using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OtherForms;

public class AdvancedFormViewModel : ViewModelBase
{
    public Settings Settings { get; } = Settings.Default;
}