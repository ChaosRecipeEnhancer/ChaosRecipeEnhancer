using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

public class RecipesFormViewModel : ViewModelBase
{
    public Settings Settings { get; } = Settings.Default;
}