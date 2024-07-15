using ChaosRecipeEnhancer.UI.Models.Config;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

class FetchOnRezoneFormViewModel : CreViewModelBase
{
    #region Fields

    private readonly IUserSettings _userSettings;

    private ICommand _selectLogFileCommand;

    #endregion

    #region Constructors

    public FetchOnRezoneFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;
    }

    #endregion

    #region Properties

    public ICommand SelectLogFileCommand => _selectLogFileCommand ??= new RelayCommand(SelectLogFile);

    public bool AutoFetchOnRezoneEnabled
    {
        get => _userSettings.AutoFetchOnRezoneEnabled;
        set
        {
            if (_userSettings.AutoFetchOnRezoneEnabled != value)
            {
                _userSettings.AutoFetchOnRezoneEnabled = value;
                OnPropertyChanged(nameof(AutoFetchOnRezoneEnabled));
            }
        }
    }

    public string PathOfExileClientLogLocation
    {
        get => _userSettings.PathOfExileClientLogLocation;
        set
        {
            if (_userSettings.PathOfExileClientLogLocation != value)
            {
                _userSettings.PathOfExileClientLogLocation = value;
                OnPropertyChanged(nameof(PathOfExileClientLogLocation));
            }
        }
    }

    public ClientLogFileLocationMode ClientLogFileLocationMode
    {
        get => (ClientLogFileLocationMode)_userSettings.PathOfExileClientLogLocationMode;
        set => UpdateClientLogFileLocationMode(value);
    }

    #endregion

    #region Methods

    public void SelectLogFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt",
            FilterIndex = 1,
            FileName = "Client.txt"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            var filename = openFileDialog.FileName;

            if (filename.EndsWith("Client.txt"))
            {
                PathOfExileClientLogLocation = filename;
            }
            else
            {
                MessageBox.Show(
                    "Invalid file selected. Make sure you're selecting the \"Client.txt\" file located in your main Path of Exile installation folder.",
                    "Missing Settings",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }

    private void UpdateClientLogFileLocationMode(ClientLogFileLocationMode mode)
    {
        if (_userSettings.PathOfExileClientLogLocationMode != (int)mode)
        {
            _userSettings.PathOfExileClientLogLocationMode = (int)mode;
            OnPropertyChanged(nameof(ClientLogFileLocationMode));

            switch (mode)
            {
                case ClientLogFileLocationMode.DefaultStandaloneLocation:
                    PathOfExileClientLogLocation = PoeClientConfigs.DefaultStandaloneInstallLocationPath;
                    break;
                case ClientLogFileLocationMode.DefaultSteamLocation:
                    PathOfExileClientLogLocation = PoeClientConfigs.DefaultSteamInstallLocationPath;
                    break;
            }
        }
    }

    #endregion
}
