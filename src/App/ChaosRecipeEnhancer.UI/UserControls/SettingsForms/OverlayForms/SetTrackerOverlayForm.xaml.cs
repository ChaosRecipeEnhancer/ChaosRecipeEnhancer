using System.Windows;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OverlayForms;

public partial class SetTrackerOverlayForm
{
    private SetTrackerOverlayFormViewModel _model;
    public SetTrackerOverlayForm()
    {
        DataContext = _model = new SetTrackerOverlayFormViewModel();
        InitializeComponent();
    }

    private void OnResetSetTrackerOverlayClicked(object sender, RoutedEventArgs e)
    {
        // setting defaults to 0,0 position (top left corner of screen)
        _model.Settings.SetTrackerOverlayTopPosition = 0;
        _model.Settings.SetTrackerOverlayLeftPosition = 0;
        // likely that the user will instantly want to reposition the overlay
        _model.Settings.SetTrackerOverlayOverlayLockPositionEnabled = false;
        _model.Settings.Save();
    }
}