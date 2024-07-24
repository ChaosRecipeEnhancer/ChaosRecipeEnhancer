using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;

public partial class LegacyAuthForm : UserControl
{
    public LegacyAuthForm()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<LegacyAuthFormViewModel>();
    }
}
