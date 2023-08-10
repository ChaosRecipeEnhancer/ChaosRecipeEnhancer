using System;
using System.Windows;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OtherForms;

public class SystemFormViewModel : ViewModelBase
{
    internal void OnAppThemeChanged()
    {
        var app = (App)Application.Current;
        app.SetApplicationTheme(Settings.AppTheme);
    }
}