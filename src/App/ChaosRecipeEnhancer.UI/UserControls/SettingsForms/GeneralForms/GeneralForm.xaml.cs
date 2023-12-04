using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.State;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
using MessageBox = System.Windows.MessageBox;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal partial class GeneralForm
{
    private readonly GeneralFormViewModel _model;
    private bool _isStashButtonCooldown = false;
    private bool _isLeaguesButtonCooldown = false;

    public GeneralForm()
    {
        InitializeComponent();
        DataContext = _model = new GeneralFormViewModel();

        // right on initialization
        _model.LoadLeagueList();
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (GlobalAuthState.Instance.ValidateLocalAuthToken()
            && _model.Settings.PoEAccountConnectionStatus == (int)ConnectionStatusTypes.ValidatedConnection
            && _model.StashTabIndexNameFullList.Count == 0)
        {
            await _model.LoadStashTabNamesIndicesAsync();
        }
    }

    private async void OnFetchStashTabsButtonClicked(object sender, RoutedEventArgs e)
    {
        if (!_isStashButtonCooldown)
        {
            // Set the cooldown status and disable the button
            _isStashButtonCooldown = true;
            FetchStashTabsButton.IsEnabled = false;

            // Perform your refresh logic here
            await _model.LoadStashTabNamesIndicesAsync();

            // Wait for the cooldown period
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Reset the cooldown status and re-enable the button
            _isStashButtonCooldown = false;
            FetchStashTabsButton.IsEnabled = true;
        }
    }

    private async void OnRefreshLeaguesButtonClicked(object sender, RoutedEventArgs e)
    {
        if (!_isLeaguesButtonCooldown)
        {
            // Set the cooldown status and disable the button
            _isLeaguesButtonCooldown = true;
            RefreshLeaguesButton.IsEnabled = false;

            // Perform your refresh logic here
            _model.LoadLeagueList();

            // Wait for the cooldown period
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Reset the cooldown status and re-enable the button
            _isLeaguesButtonCooldown = false;
            RefreshLeaguesButton.IsEnabled = true;
        }
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
}