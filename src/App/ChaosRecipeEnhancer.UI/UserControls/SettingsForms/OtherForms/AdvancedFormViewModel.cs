﻿using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Services;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Windows;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OtherForms;

public class AdvancedFormViewModel : CreViewModelBase
{
    private readonly ILogger _log = Log.ForContext<AdvancedFormViewModel>();
    private readonly IAuthStateManager _authStateManager;
    private readonly IUserSettings _userSettings;
    private ICommand _resetSettingsCommand;

    public AdvancedFormViewModel(IAuthStateManager authStateManager, IUserSettings userSettings)
    {
        _authStateManager = authStateManager;
        _userSettings = userSettings;
    }

    #region Properties

    public ICommand ResetSettingsCommand => _resetSettingsCommand ??= new RelayCommand(ResetSettings);

    public bool DoNotPreserveLowItemLevelGearIsChecked
    {
        get => _userSettings.DoNotPreserveLowItemLevelGear;
        set
        {
            _userSettings.DoNotPreserveLowItemLevelGear = value;
            OnPropertyChanged();
        }
    }

    public bool LegacyAuthModeIsChecked
    {
        get => _userSettings.LegacyAuthMode;
        set => LegacyAuthModeUpdated(value);
    }

    public bool DebugModeIsChecked
    {
        get => _userSettings.DebugMode;
        set
        {
            _userSettings.DebugMode = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Methods

    private void ResetSettings()
    {
        _log.Warning("User initiated reset settings");

        switch (MessageBox.Show("This will reset all of your settings. You will lose all your user configurations if decide not to manually back them up.\n\nThe application will shut down after this for all application state to be reset.", "Warning: Reset Settings", MessageBoxButton.YesNo))
        {
            case MessageBoxResult.Yes:
                _log.Information("User confirmed reset settings");

                // Reset settings to default
                _userSettings.Reset();
                _log.Information("User settings reset to default");

                // Reset some local application state (to avoid issues with out-of-sync state stored in memory)
                _authStateManager.Logout();
                _log.Information("User logged out");

                GlobalHotkeyState.RemoveAllHotkeys();
                _log.Information("Global hotkeys removed");

                Application.Current.Shutdown();

                break;
            case MessageBoxResult.No:
                _log.Information("User canceled reset settings");
                break;
        }
    }

    private void LegacyAuthModeUpdated(bool value)
    {
        _userSettings.LegacyAuthMode = value;

        // if the user is changing this we need to clear our stash indices
        _userSettings.StashTabIds = [];

        OnPropertyChanged();
    }

    #endregion
}