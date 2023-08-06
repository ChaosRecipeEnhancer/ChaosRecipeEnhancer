using System.ComponentModel;
using System.Windows;
using ChaosRecipeEnhancer.UI.BusinessLogic.DataFetching;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms
{
    public partial class PathOfExileForm : INotifyPropertyChanged
    {
        private Visibility _mainLeagueVisible = Visibility.Visible;
        private Visibility _customLeagueVisible = Visibility.Hidden;

        public PathOfExileForm()
        {
            DataContext = this;
            InitializeComponent();

            // Populate the league dropdown
            if (!Settings.Default.CustomLeagueEnabled) MainLeagueComboBox.ItemsSource = ApiAdapter.FetchLeagueNames();

            LoadCustomLeagueInputVisibility();
        }

        public Visibility MainLeagueVisible
        {
            get => _mainLeagueVisible;
            set
            {
                if (_mainLeagueVisible != value)
                {
                    _mainLeagueVisible = value;
                    OnPropertyChanged("MainLeagueVisible");
                }
            }
        }

        public Visibility CustomLeagueVisible
        {
            get => _customLeagueVisible;
            set
            {
                if (_customLeagueVisible != value)
                {
                    _customLeagueVisible = value;
                    OnPropertyChanged("CustomLeagueVisible");
                }
            }
        }

        private void CustomLeagueCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.Default.CustomLeagueEnabled = true;
            LoadCustomLeagueInputVisibility();
        }

        private void CustomLeagueCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.Default.CustomLeagueEnabled = false;
            LoadCustomLeagueInputVisibility();
        }

        private void LoadCustomLeagueInputVisibility()
        {
            if (!Settings.Default.CustomLeagueEnabled)
            {
                CustomLeagueVisible = Visibility.Hidden;
                MainLeagueVisible = Visibility.Visible;
            }
            else
            {
                CustomLeagueVisible = Visibility.Visible;
                MainLeagueVisible = Visibility.Hidden;
            }
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
}