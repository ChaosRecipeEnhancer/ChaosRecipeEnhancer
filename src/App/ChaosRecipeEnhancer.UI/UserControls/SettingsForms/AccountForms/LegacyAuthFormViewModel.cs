﻿using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.Exceptions;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Services;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;

public class LegacyAuthFormViewModel : CreViewModelBase
{
    #region Fields

    private readonly ILogger _log = Log.ForContext<LegacyAuthFormViewModel>();
    private readonly IAuthStateManager _authStateManager;
    private readonly IPoeApiService _poeApiService;
    private readonly IUserSettings _userSettings;

    private ICommand _loginCommand;

    private Visibility _connectionNotValidatedTextVisibility;
    private Visibility _loggedInTextVisibility;
    private Visibility _connectionErrorTextVisibility;

    #endregion

    #region Constructors

    public LegacyAuthFormViewModel(IAuthStateManager authState, IPoeApiService poeApiService, IUserSettings userSettings)
    {
        _authStateManager = authState;
        _poeApiService = poeApiService;
        _userSettings = userSettings;

        _authStateManager.AuthStateChanged += AuthStateManager_AuthStateChanged;
    }

    ~LegacyAuthFormViewModel()
    {
        // Unsubscribe to avoid memory leaks
        _authStateManager.AuthStateChanged -= AuthStateManager_AuthStateChanged;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the command to initiate the login process.
    /// </summary>
    public ICommand LoginCommand => _loginCommand ??= new AsyncRelayCommand(Login);

    /// <summary>
    /// Gets the visibility of the "Connection Not Validated" text.
    /// </summary>
    public Visibility ConnectionNotValidatedTextVisibility
    {
        get => GlobalUserSettings.PoEAccountConnectionStatus == (int)ConnectionStatusTypes.ConnectionNotValidated ? Visibility.Visible : Visibility.Collapsed;
        private set => SetProperty(ref _connectionNotValidatedTextVisibility, value);
    }

    /// <summary>
    /// Gets the visibility of the "Logged In" text.
    /// </summary>
    public Visibility LoggedInTextVisibility
    {
        get => GlobalUserSettings.PoEAccountConnectionStatus == (int)ConnectionStatusTypes.ValidatedConnection ? Visibility.Visible : Visibility.Collapsed;
        private set => SetProperty(ref _loggedInTextVisibility, value);
    }

    /// <summary>
    /// Gets the visibility of the "Connection Error" text.
    /// </summary>
    public Visibility ConnectionErrorTextVisibility
    {
        get => GlobalUserSettings.PoEAccountConnectionStatus == (int)ConnectionStatusTypes.ConnectionError ? Visibility.Visible : Visibility.Collapsed;
        private set => SetProperty(ref _connectionErrorTextVisibility, value);
    }

    public string LegacyAuthSessionId
    {
        get => _userSettings.LegacyAuthSessionId;
        set
        {
            if (_userSettings.LegacyAuthSessionId != value)
            {
                _userSettings.LegacyAuthSessionId = value;
                OnPropertyChanged(nameof(LegacyAuthSessionId));
            }
        }
    }

    #endregion

    #region Methods

    public async Task Login()
    {
        try
        {
            _log.Information("Starting the session id validation process...");
            GlobalUserSettings.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.AttemptingLogin;
            OnPropertyChanged(nameof(ConnectionNotValidatedTextVisibility));

            // Perform a health check using PoeApiService
            var isValid = await ValidateCredentials();

            if (isValid)
            {
                GlobalUserSettings.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ValidatedConnection;
                _log.Information("Validation process completed successfully.");
            }
        }
        catch (UnauthorizedException)
        {
            GlobalUserSettings.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ConnectionNotValidated;
            _log.Warning("Validation failed: Unauthorized access");
        }
        catch (Exception ex)
        {
            GlobalUserSettings.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ConnectionError;
            _log.Error(ex, "An error occurred during the validation process.");
        }
        finally
        {
            NotifyAllPropertiesChanged();
        }
    }

    private async Task<bool> ValidateCredentials()
    {
        try
        {
            var healthCheckResponse = await _poeApiService.HealthCheckWithSesionIdAsync(_userSettings.LegacyAuthSessionId);
            return healthCheckResponse != null && healthCheckResponse.Total > 0;
        }
        catch (UnauthorizedException)
        {
            // Re-throw the exception to be caught in the Login method
            throw;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error validating credentials");
            return false;
        }
    }

    private void AuthStateManager_AuthStateChanged(object sender, EventArgs e)
    {
        _log.Information("Auth state changed, updating UI...");
        NotifyAllPropertiesChanged();
    }

    private void NotifyAllPropertiesChanged()
    {
        OnPropertyChanged(nameof(ConnectionNotValidatedTextVisibility));
        OnPropertyChanged(nameof(LoggedInTextVisibility));
        OnPropertyChanged(nameof(ConnectionErrorTextVisibility));
    }

    #endregion
}