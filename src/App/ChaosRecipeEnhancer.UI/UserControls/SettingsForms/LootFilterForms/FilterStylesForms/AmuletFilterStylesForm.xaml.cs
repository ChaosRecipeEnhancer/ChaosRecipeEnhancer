using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public partial class AmuletFilterStylesForm : UserControl
{
    private readonly AmuletFilterStylesFormViewModel _model;

    public AmuletFilterStylesForm()
    {
        DataContext = _model = Ioc.Default.GetService<AmuletFilterStylesFormViewModel>();
        InitializeComponent();
    }
}
