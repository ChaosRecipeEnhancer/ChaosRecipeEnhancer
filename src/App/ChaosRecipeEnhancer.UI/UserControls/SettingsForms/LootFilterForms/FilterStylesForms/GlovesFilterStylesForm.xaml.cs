using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public partial class GlovesFilterStylesForm : UserControl
{
    private readonly GlovesFilterStylesFormViewModel _model;

    public GlovesFilterStylesForm()
    {
        DataContext = _model = Ioc.Default.GetService<GlovesFilterStylesFormViewModel>();
        InitializeComponent();
    }
}
