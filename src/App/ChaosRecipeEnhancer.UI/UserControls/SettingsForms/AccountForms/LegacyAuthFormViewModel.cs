using ChaosRecipeEnhancer.UI.Models.Enums;
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
        get => PoEAccountConnectionStatus == ConnectionStatusTypes.ConnectionNotValidated ? Visibility.Visible : Visibility.Collapsed;
        private set => SetProperty(ref _connectionNotValidatedTextVisibility, value);
    }

    /// <summary>
    /// Gets the visibility of the "Logged In" text.
    /// </summary>
    public Visibility LoggedInTextVisibility
    {
        get => PoEAccountConnectionStatus == ConnectionStatusTypes.ValidatedConnection ? Visibility.Visible : Visibility.Collapsed;
        private set => SetProperty(ref _loggedInTextVisibility, value);
    }

    /// <summary>
    /// Gets the visibility of the "Connection Error" text.
    /// </summary>
    public Visibility ConnectionErrorTextVisibility
    {
        get => PoEAccountConnectionStatus == ConnectionStatusTypes.ConnectionError ? Visibility.Visible : Visibility.Collapsed;
        private set => SetProperty(ref _connectionErrorTextVisibility, value);
    }

    public ConnectionStatusTypes PoEAccountConnectionStatus
    {
        get => _userSettings.PoEAccountConnectionStatus;
        set
        {
            if (_userSettings.PoEAccountConnectionStatus != value)
            {
                _userSettings.PoEAccountConnectionStatus = value;
                OnPropertyChanged(nameof(PoEAccountConnectionStatus));
            }
        }
    }

    public string LegacyAuthAccountName
    {
        get => _userSettings.LegacyAuthAccountName;
        set
        {
            if (_userSettings.LegacyAuthAccountName != value)
            {
                _userSettings.LegacyAuthAccountName = value;
                OnPropertyChanged(nameof(LegacyAuthAccountName));
            }
        }
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
            PoEAccountConnectionStatus = ConnectionStatusTypes.AttemptingLogin;
            OnPropertyChanged(nameof(ConnectionNotValidatedTextVisibility));

            // Perform a health check using PoeApiService
            var isValid = await ValidateCredentials();

            if (isValid)
            {
                PoEAccountConnectionStatus = ConnectionStatusTypes.ValidatedConnection;
                _log.Information("Validation process completed successfully.");
            }
        }
        catch (UnauthorizedException)
        {
            PoEAccountConnectionStatus = ConnectionStatusTypes.ConnectionNotValidated;
            _log.Warning("Validation failed: Unauthorized access");
        }
        catch (Exception ex)
        {
            PoEAccountConnectionStatus = ConnectionStatusTypes.ConnectionError;
            _log.Error(ex, "An error occurred during the validation process.");
        }
        finally
        {
            OnPropertyChanged(string.Empty);
        }
    }

    private async Task<bool> ValidateCredentials()
    {
        try
        {
            var healthCheckResponse = await _poeApiService.HealthCheckWithSesionIdAsync(LegacyAuthSessionId);
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
        OnPropertyChanged(string.Empty);
    }

    #endregion
}