namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OtherForms;

public partial class SystemForm
{
    public SystemForm()
    {
        DataContext = new SystemFormViewModel();
        InitializeComponent();
    }
}