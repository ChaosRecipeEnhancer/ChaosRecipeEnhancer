using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

public partial class FetchOnRezoneForm : UserControl
{
    private readonly FetchOnRezoneFormViewModel _model;

    public FetchOnRezoneForm()
    {
        InitializeComponent();
        DataContext = _model = Ioc.Default.GetService<FetchOnRezoneFormViewModel>();
    }
}
