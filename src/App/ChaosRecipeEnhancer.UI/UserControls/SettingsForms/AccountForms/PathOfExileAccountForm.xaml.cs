using System.Windows;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;

internal partial class PathOfExileAccountForm
{
    private readonly PathOfExileAccountFormViewModel _model;

    public PathOfExileAccountForm()
    {
        InitializeComponent();
        DataContext = _model = new PathOfExileAccountFormViewModel();

        // right on initialization
        _model.TestConnectionToPoEServers();
    }

    private void OnTestConnectionClicked(object sender, RoutedEventArgs e)
    {
        _model.TestConnectionToPoEServers();
    }
}