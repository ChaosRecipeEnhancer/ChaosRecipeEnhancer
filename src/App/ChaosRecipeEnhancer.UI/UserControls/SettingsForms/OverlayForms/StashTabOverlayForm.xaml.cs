using System.Windows;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OverlayForms;

public partial class StashTabOverlayForm
{
    public StashTabOverlayForm()
    {
        DataContext = new StashTabOverlayFormViewModel();
        InitializeComponent();
    }
}