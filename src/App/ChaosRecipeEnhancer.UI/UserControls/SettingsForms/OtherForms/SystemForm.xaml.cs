using CommunityToolkit.Mvvm.DependencyInjection;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OtherForms;

public partial class SystemForm
{
    public SystemForm()
    {
        DataContext = Ioc.Default.GetService<SystemFormViewModel>();
        InitializeComponent();
    }
}