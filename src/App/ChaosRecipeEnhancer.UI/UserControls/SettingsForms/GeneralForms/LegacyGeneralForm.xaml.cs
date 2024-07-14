using ChaosRecipeEnhancer.UI.Properties;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.Windows;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

/// <summary>
/// Interaction logic for LegacyGeneralForm.xaml
/// </summary>
public partial class LegacyGeneralForm
{
    private readonly LegacyGeneralFormViewModel _model;

    public LegacyGeneralForm()
    {
        InitializeComponent();
        DataContext = _model = Ioc.Default.GetService<LegacyGeneralFormViewModel>();
    }

    private async void OnFetchStashTabsButtonClicked(object sender, RoutedEventArgs e)
    {
        await _model.LoadStashTabNamesIndicesAsync();
    }

    private void OnRefreshLeaguesButtonClicked(object sender, RoutedEventArgs e)
    {
        _model.LoadLeagueList();
    }

    private void OnStashTabSelectionChanged(object sender, ItemSelectionChangedEventArgs itemSelectionChangedEventArgs)
    {
        var checkComboBox = (CheckComboBox)sender;
        _model.UpdateSelectedTabList(checkComboBox.SelectedItems);
    }

    private static bool CheckAccountSettings(bool showError)
    {
        var missingSettings = new List<string>();
        var errorMessage = "Please add: \n";

        if (string.IsNullOrEmpty(Settings.Default.LegacyAuthAccountName)) missingSettings.Add("- Account Name \n");
        if (string.IsNullOrEmpty(Settings.Default.LegacyAuthSessionId)) missingSettings.Add("- PoE Session ID \n");
        if (string.IsNullOrEmpty(Settings.Default.LeagueName)) missingSettings.Add("- League \n");

        if (missingSettings.Count == 0) return true;

        foreach (var setting in missingSettings) errorMessage += setting;

        if (showError) _ = System.Windows.MessageBox.Show(errorMessage, "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);

        return false;
    }
}
