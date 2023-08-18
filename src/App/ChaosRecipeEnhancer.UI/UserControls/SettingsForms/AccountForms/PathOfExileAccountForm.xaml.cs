using System.ComponentModel;
using System.Windows;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;

internal partial class PathOfExileAccountForm
{
    private readonly PathOfExileAccountFormViewModel _model;

    public PathOfExileAccountForm()
    {
        InitializeComponent();
        DataContext = _model = new PathOfExileAccountFormViewModel();

        Settings.Default.PropertyChanged += OnSettingsChanged;
    }

    private void OnTestConnectionClicked(object sender, RoutedEventArgs e)
    {
        _model.TestConnectionToPoEServers();

    }

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.PathOfExileWebsiteSessionId) ||
            e.PropertyName == nameof(Settings.PathOfExileAccountName))
        {
            _model.Settings.PoEAccountConnectionStatus = 0;
            _model.Settings.Save();
        }
    }
}