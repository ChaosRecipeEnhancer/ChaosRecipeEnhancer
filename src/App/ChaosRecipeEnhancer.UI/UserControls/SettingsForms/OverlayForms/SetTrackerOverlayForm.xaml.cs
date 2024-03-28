using CommunityToolkit.Mvvm.DependencyInjection;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OverlayForms;

public partial class SetTrackerOverlayForm
{
    public SetTrackerOverlayForm()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetService<SetTrackerOverlayFormViewModel>();
    }
}