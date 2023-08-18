using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
using MessageBox = System.Windows.MessageBox;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal partial class GeneralForm
{
    private readonly GeneralFormViewModel _model;

    public GeneralForm()
    {
        InitializeComponent();
        DataContext = _model = new GeneralFormViewModel();

        // right on initialization
        _model.LoadLeagueList();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (CheckAccountSettings(false)
            && _model.Settings.PoEAccountConnectionStatus == (int)ConnectionStatusTypes.ValidatedConnection
            && _model.StashTabIndexNameFullList.Count == 0)
        {
            await _model.LoadStashTabNamesIndicesAsync();
        }
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

    private void LogLocationDialog_Click(object sender, RoutedEventArgs e)
    {
        var open = new OpenFileDialog();
        open.Filter = "Text|Client.txt";
        var res = open.ShowDialog();

        if (res != DialogResult.OK) return;

        var filename = open.FileName;

        if (filename.EndsWith("Client.txt"))
        {
            Settings.Default.PathOfExileClientLogLocation = filename;
            LogLocationDialog.Content = filename;
        }
        else
        {
            MessageBox.Show(
                "Invalid file selected. Make sure you're selecting the \"Client.txt\" file located in your main Path of Exile installation folder.",
                "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static bool CheckAccountSettings(bool showError)
    {
        var missingSettings = new List<string>();
        var errorMessage = "Please add: \n";

        if (string.IsNullOrEmpty(Settings.Default.PathOfExileAccountName)) missingSettings.Add("- Account Name \n");
        if (string.IsNullOrEmpty(Settings.Default.PathOfExileWebsiteSessionId)) missingSettings.Add("- PoE Session ID \n");
        if (string.IsNullOrEmpty(Settings.Default.LeagueName)) missingSettings.Add("- League \n");

        if (missingSettings.Count == 0) return true;

        foreach (var setting in missingSettings) errorMessage += setting;

        if (showError) _ = MessageBox.Show(errorMessage, "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);

        return false;
    }
}