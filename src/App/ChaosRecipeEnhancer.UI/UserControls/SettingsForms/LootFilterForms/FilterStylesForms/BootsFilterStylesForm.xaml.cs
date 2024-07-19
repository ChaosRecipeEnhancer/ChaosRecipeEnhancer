using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public partial class BootsFilterStylesForm : UserControl
{
    private readonly BootsFilterStylesFormViewModel _model;

    public BootsFilterStylesForm()
    {
        DataContext = _model = Ioc.Default.GetService<BootsFilterStylesFormViewModel>();
        InitializeComponent();
    }
}
