using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OverlayForms;

public class SetTrackerOverlayFormViewModel : CreViewModelBase
{
    #region Fields

    private readonly IUserSettings _userSettings;
    private ICommand _resetSetTrackerOverlayPositionCommand;

    #endregion

    #region Constructors

    public SetTrackerOverlayFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;
    }

    #endregion

    #region Properties

    public ICommand ResetSetTrackerOverlayPositionCommand => _resetSetTrackerOverlayPositionCommand ??= new RelayCommand(ResetSetTrackerOverlayPosition);

    #region User Settings Properties

    // HACK: This is a hack to get around the fact that the SetTrackerOverlayItemCounterDisplayMode
    // enum is not able to be bound to a ComboBox.SelectedIndex property.
    public int SetTrackerOverlayItemCounterDisplayMode
    {
        get => (int)_userSettings.SetTrackerOverlayItemCounterDisplayMode;
        set
        {
            if ((int)_userSettings.SetTrackerOverlayItemCounterDisplayMode != value)
            {
                _userSettings.SetTrackerOverlayItemCounterDisplayMode = (SetTrackerOverlayItemCounterDisplayMode)value;
                OnPropertyChanged(nameof(SetTrackerOverlayItemCounterDisplayMode));
            }
        }
    }

    // HACK: This is a hack to get around the fact that the SetTrackerOverlayMode enum
    // is not able to be bound to a ComboBox.SelectedIndex property.
    public int SetTrackerOverlayMode
    {
        get => (int)_userSettings.SetTrackerOverlayMode;
        set
        {
            if ((int)_userSettings.SetTrackerOverlayMode != value)
            {
                _userSettings.SetTrackerOverlayMode = (SetTrackerOverlayMode)value;
                OnPropertyChanged(nameof(SetTrackerOverlayMode));
            }
        }
    }

    public bool SetTrackerOverlayOverlayLockPositionEnabled
    {
        get => _userSettings.SetTrackerOverlayOverlayLockPositionEnabled;
        set
        {
            if (_userSettings.SetTrackerOverlayOverlayLockPositionEnabled != value)
            {
                _userSettings.SetTrackerOverlayOverlayLockPositionEnabled = value;
                OnPropertyChanged(nameof(SetTrackerOverlayOverlayLockPositionEnabled));
            }
        }
    }

    public double SetTrackerOverlayTopPosition
    {
        get => _userSettings.SetTrackerOverlayTopPosition;
        set
        {
            if (_userSettings.SetTrackerOverlayTopPosition != value)
            {
                _userSettings.SetTrackerOverlayTopPosition = value;
                OnPropertyChanged(nameof(SetTrackerOverlayTopPosition));
            }
        }
    }

    public double SetTrackerOverlayLeftPosition
    {
        get => _userSettings.SetTrackerOverlayLeftPosition;
        set
        {
            if (_userSettings.SetTrackerOverlayLeftPosition != value)
            {
                _userSettings.SetTrackerOverlayLeftPosition = value;
                OnPropertyChanged(nameof(SetTrackerOverlayLeftPosition));
            }
        }
    }

    public bool SilenceSetsFullMessage
    {
        get => _userSettings.SilenceSetsFullMessage;
        set
        {
            if (_userSettings.SilenceSetsFullMessage != value)
            {
                _userSettings.SilenceSetsFullMessage = value;
                OnPropertyChanged(nameof(SilenceSetsFullMessage));
            }
        }
    }

    public bool SilenceNeedItemsMessage
    {
        get => _userSettings.SilenceNeedItemsMessage;
        set
        {
            if (_userSettings.SilenceNeedItemsMessage != value)
            {
                _userSettings.SilenceNeedItemsMessage = value;
                OnPropertyChanged(nameof(SilenceNeedItemsMessage));
            }
        }
    }

    #endregion

    #endregion

    #region Methods

    private void ResetSetTrackerOverlayPosition()
    {
        SetTrackerOverlayTopPosition = 0;
        SetTrackerOverlayLeftPosition = 0;
        SetTrackerOverlayOverlayLockPositionEnabled = false;
    }

    #endregion
}