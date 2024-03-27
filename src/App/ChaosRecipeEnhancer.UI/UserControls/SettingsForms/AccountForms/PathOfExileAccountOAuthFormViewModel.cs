using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.State;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System;
using System.Windows;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;

public class PathOfExileAccountOAuthFormViewModel : CreViewModelBase
{
    #region Fields

    private readonly ILogger _log = Log.ForContext<PathOfExileAccountOAuthFormViewModel>();
    private readonly IAuthStateManager _authStateManager;
    private ICommand _loginCommand;
    private ICommand _logoutCommand;
    private Visibility _connectionNotValidatedTextVisibility;
    private Visibility _loggedInTextVisibility;
    private Visibility _connectionErrorTextVisibility;
    private Visibility _attemptingLoginTextVisibility;
    private Visibility _loginButtonVisibility;
    private Visibility _logoutButtonVisibility;

    #endregion

    #region Constructors

    public PathOfExileAccountOAuthFormViewModel(IAuthStateManager authState)
    {
        _authStateManager = authState;

        _authStateManager.AuthStateChanged += AuthStateManager_AuthStateChanged;
    }

    ~PathOfExileAccountOAuthFormViewModel()
    {
        // Unsubscribe to avoid memory leaks
        _authStateManager.AuthStateChanged -= AuthStateManager_AuthStateChanged;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the command to initiate the login process.
    /// </summary>
    public ICommand LoginCommand => _loginCommand ??= new RelayCommand(Login);

    /// <summary>
    /// Gets the command to initiate the logout process.
    /// </summary>
    public ICommand LogoutCommand => _logoutCommand ??= new RelayCommand(Logout);

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

    /// <summary>
    /// Gets the visibility of the "Attempting Login" text.
    /// </summary>
    public Visibility AttemptingLoginTextVisibility
    {
        get => GlobalUserSettings.PoEAccountConnectionStatus == (int)ConnectionStatusTypes.AttemptingLogin ? Visibility.Visible : Visibility.Collapsed;
        private set => SetProperty(ref _attemptingLoginTextVisibility, value);
    }

    /// <summary>
    /// Gets the visibility of the login button.
    /// </summary>
    public Visibility LoginButtonVisibility
    {
        get => GlobalUserSettings.PoEAccountConnectionStatus != (int)ConnectionStatusTypes.ValidatedConnection ? Visibility.Visible : Visibility.Collapsed;
        private set => SetProperty(ref _loginButtonVisibility, value);
    }

    /// <summary>
    /// Gets the visibility of the logout button.
    /// </summary>
    public Visibility LogoutButtonVisibility
    {
        get => GlobalUserSettings.PoEAccountConnectionStatus == (int)ConnectionStatusTypes.ValidatedConnection ? Visibility.Visible : Visibility.Collapsed;
        private set => SetProperty(ref _logoutButtonVisibility, value);
    }

    #endregion

    #region Methods

    public void Login()
    {
        try
        {
            _log.Information("Starting the login process...");
            _authStateManager.Login();
            _log.Information("Login process completed successfully.");
        }
        catch (Exception ex)
        {
            _log.Error(ex, "An error occurred during the login process.");
        }
    }

    public void Logout()
    {
        try
        {
            _log.Information("Starting the logout process...");
            _authStateManager.Logout();
            _log.Information("Logout process completed successfully.");
        }
        catch (Exception ex)
        {
            _log.Error(ex, "An error occurred during the logout process.");
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
        OnPropertyChanged(nameof(AttemptingLoginTextVisibility));
        OnPropertyChanged(nameof(LoginButtonVisibility));
        OnPropertyChanged(nameof(LogoutButtonVisibility));
    }

    #endregion
}
