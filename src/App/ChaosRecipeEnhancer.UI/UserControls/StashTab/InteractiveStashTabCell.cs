using System.ComponentModel;
using ChaosRecipeEnhancer.UI.Models;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTab;

public sealed class InteractiveStashTabCell : INotifyPropertyChanged
{
    private bool _active;
    private string _buttonText;

    /// <summary>
    /// Representation of PoE item class (translated directly from PoE API request). This is the in-game item the
    /// current cell (visual object/button) is tied to.
    /// </summary>
    public EnhancedItem ItemModel { get; set; }

    public int XIndex { get; set; }
    public int YIndex { get; set; }

    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            OnPropertyChanged("Active");
        }
    }

    public string ButtonText
    {
        get => _buttonText;
        set
        {
            _buttonText = value;
            OnPropertyChanged("ButtonText");
        }
    }

    #region INotifyPropertyChanged implementation

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}