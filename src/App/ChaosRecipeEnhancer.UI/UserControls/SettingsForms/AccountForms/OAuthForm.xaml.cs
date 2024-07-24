using CommunityToolkit.Mvvm.DependencyInjection;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;

public partial class OAuthForm : UserControl
{
    public OAuthForm()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<OAuthFormViewModel>();
    }
}
