using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Properties;
using MessageBox = System.Windows.MessageBox;

// using ChaosRecipeEnhancer.UI.BusinessLogic.DataFetching;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal partial class GeneralForm : INotifyPropertyChanged
{
    private Visibility _fetchOnNewMapEnabled = Visibility.Collapsed;
    private Visibility _tabIndicesVisible = Visibility.Visible;
    private Visibility _tabNamePrefixVisible = Visibility.Hidden;
    private Visibility _tabNameSuffixVisible = Visibility.Hidden;

    public GeneralForm()
    {
        DataContext = this;
        InitializeComponent();
        LoadStashQueryModeVisibility();
        LoadFetchOnNewMapEnabled();
    }

    public Visibility TabIndicesVisible
    {
        get => _tabIndicesVisible;
        set
        {
            if (_tabIndicesVisible != value)
            {
                _tabIndicesVisible = value;
                OnPropertyChanged("TabIndicesVisible");
            }
        }
    }

    public Visibility TabNamePrefixVisible
    {
        get => _tabNamePrefixVisible;
        set
        {
            if (_tabNamePrefixVisible != value)
            {
                _tabNamePrefixVisible = value;
                OnPropertyChanged("TabNamePrefixVisible");
            }
        }
    }

    public Visibility TabNameSuffixVisible
    {
        get => _tabNameSuffixVisible;
        set
        {
            if (_tabNameSuffixVisible != value)
            {
                _tabNameSuffixVisible = value;
                OnPropertyChanged("TabNameSuffixVisible");
            }
        }
    }

    public Visibility FetchOnNewMapEnabled
    {
        get => _fetchOnNewMapEnabled;
        set
        {
            if (_fetchOnNewMapEnabled != value)
            {
                _fetchOnNewMapEnabled = value;
                OnPropertyChanged("FetchOnNewMapEnabled");
            }
        }
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadStashQueryModeVisibility();
    }

    private void StashTargetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }

    private void AutoFetchCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        LoadFetchOnNewMapEnabled();
    }

    private void AutoFetchCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        LoadFetchOnNewMapEnabled();

        // if (LogWatcher.WorkerThread != null && LogWatcher.WorkerThread.IsAlive) LogWatcher.WorkerThread.Interrupt();
    }

    private void LogLocationDialog_Click(object sender, RoutedEventArgs e)
    {
        var open = new OpenFileDialog();
        open.Filter = "Text|Client.txt";
        var res = open.ShowDialog();

        if (res != DialogResult.OK) return;

        var filename = open.FileName;

        if (filename.EndsWith("Client.txt"))
        {
            Settings.Default.PathOfExileClientLogLocation = filename;
            LogLocationDialog.Content = filename;
        }
        else
        {
            MessageBox.Show(
                "Invalid file selected. Make sure you're selecting the \"Client.txt\" file located in your main Path of Exile installation folder.",
                "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadStashQueryModeVisibility()
    {
        switch (Settings.Default.StashTabQueryMode)
        {
            case 0:
                TabIndicesVisible = Visibility.Visible;
                TabNamePrefixVisible = Visibility.Hidden;
                TabNameSuffixVisible = Visibility.Hidden;
                break;
            case 1:
                TabIndicesVisible = Visibility.Hidden;
                TabNamePrefixVisible = Visibility.Visible;
                TabNameSuffixVisible = Visibility.Hidden;
                break;
            case 2:
                TabIndicesVisible = Visibility.Hidden;
                TabNamePrefixVisible = Visibility.Hidden;
                TabNameSuffixVisible = Visibility.Visible;
                break;
        }
    }

    private void LoadFetchOnNewMapEnabled()
    {
        FetchOnNewMapEnabled = Settings.Default.AutoFetchOnRezoneEnabled
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    #region INotifyPropertyChanged implementation

    // Basically, the UI thread subscribes to this event and update the binding if the received Property Name correspond to the Binding Path element
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        if (handler != null)
            handler(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}