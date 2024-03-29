using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Services;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OtherForms;

public class SystemFormViewModel : CreViewModelBase
{
    private readonly INotificationSoundService _notificationSoundService;
    private readonly IUserSettings _userSettings;

    public SystemFormViewModel(IUserSettings userSettings, INotificationSoundService notificationSoundService)
    {
        _userSettings = userSettings;
        _notificationSoundService = notificationSoundService;
        TestSoundCommand = new RelayCommand(TestNotificationSound);
    }

    public ICommand TestSoundCommand { get; }

    public bool SoundEnabled
    {
        get => _userSettings.SoundEnabled;
        set
        {
            _userSettings.SoundEnabled = value;
            OnPropertyChanged(nameof(SoundEnabled));
        }
    }

    public double SoundLevel
    {
        get => _userSettings.SoundLevel;
        set
        {
            _userSettings.SoundLevel = value;
            OnPropertyChanged(nameof(SoundLevel));
        }
    }

    public int Language
    {
        get => (int)_userSettings.Language;
        set
        {
            _userSettings.Language = (Language)value;
            OnPropertyChanged(nameof(Language));
        }
    }

    public bool CloseToTrayEnabled
    {
        get => _userSettings.CloseToTrayEnabled;
        set
        {
            _userSettings.CloseToTrayEnabled = value;
            OnPropertyChanged(nameof(CloseToTrayEnabled));
        }
    }


    public void TestNotificationSound()
    {
        _notificationSoundService.PlayNotificationSound(NotificationSoundType.ItemSetStateChanged);
    }
}