using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public partial class HelmetFilterStylesForm : UserControl
{
    private readonly HelmetFilterStylesFormViewModel _model;

    public HelmetFilterStylesForm()
    {
        DataContext = _model = Ioc.Default.GetService<HelmetFilterStylesFormViewModel>();
        InitializeComponent();
    }
}
