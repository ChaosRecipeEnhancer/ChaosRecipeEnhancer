using CommunityToolkit.Mvvm.DependencyInjection;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OtherForms;

public partial class AdvancedForm
{
    public AdvancedForm()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<AdvancedFormViewModel>();
    }
}