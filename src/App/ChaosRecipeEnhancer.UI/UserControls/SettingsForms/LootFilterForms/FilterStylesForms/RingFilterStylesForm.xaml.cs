using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public partial class RingFilterStylesForm : UserControl
{
    private readonly RingFilterStylesFormViewModel _model;

    public RingFilterStylesForm()
    {
        DataContext = _model = Ioc.Default.GetService<RingFilterStylesFormViewModel>();
        InitializeComponent();
    }
}
