namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal partial class RecipesForm
{
    public RecipesForm()
    {
        DataContext = new RecipesFormViewModel();
        InitializeComponent();
    }
}