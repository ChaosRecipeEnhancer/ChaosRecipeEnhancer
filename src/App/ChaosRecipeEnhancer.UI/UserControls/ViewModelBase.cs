using ChaosRecipeEnhancer.UI.Properties;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ChaosRecipeEnhancer.UI.UserControls;

// TODO: consolidate our base viewmodel - this will eventually be depracated in favor of CreViewModelBase
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public Settings GlobalUserSettings { get; } = Settings.Default;
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T property, T newValue, [CallerMemberName] string propertyName = null)
    {
        if (Equals(property, newValue)) return false;
        property = newValue;
        OnPropertyChanged(propertyName);
        return true;
    }
}