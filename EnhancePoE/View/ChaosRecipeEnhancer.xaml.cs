using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using EnhancePoE.Model;
using EnhancePoE.Properties;

namespace EnhancePoE
{
    /// <summary>
    ///     Interaction logic for ChaosRecipeEnhancer.xaml
    /// </summary>
    public partial class ChaosRecipeEnhancer : INotifyPropertyChanged
    {
        private const double DeactivatedOpacity = .1;
        private const double ActivatedOpacity = 1;
        private const int FetchCooldown = 30;

        private string _openStashOverlayButtonContent = "Stash";
        private double _shadowOpacity;
        private string _warningMessage;
        private bool _isIndeterminate;
        private bool _fetchButtonEnabled = true;
        private SolidColorBrush _fetchButtonColor = Brushes.Green;
        private Visibility _amountsVisibility = Visibility.Hidden;
        private Visibility _warningMessageVisibility = Visibility.Hidden;

        // Defines the number of a given piece of gear you currently have
        private int _amuletsAmount;
        private int _ringsAmount;
        private int _beltsAmount;
        private int _helmetsAmount;
        private int _glovesAmount;
        private int _bootsAmount;
        private int _chestsAmount;
        private int _weaponsAmount;
        
        // Defines whether or not to 'Activate' a gear icon based on the full set threshold
        // Upon initialization, the icons are clearly visible (i.e. 'Activated', not translucent at all)
        private double _amuletsOpacity = ActivatedOpacity;
        private double _ringsOpacity = ActivatedOpacity;
        private double _beltsOpacity = ActivatedOpacity;
        private double _helmetOpacity = ActivatedOpacity;
        private double _glovesOpacity = ActivatedOpacity;
        private double _bootsOpacity = ActivatedOpacity;
        private double _chestsOpacity = ActivatedOpacity;
        private double _weaponsOpacity = ActivatedOpacity;

        private string _fullSetsText = "0";
        
        public ChaosRecipeEnhancer()
        {
            InitializeComponent();
            DataContext = this;
            FullSetsText = "0";
        }

        // Tracks whether or not we're currently fetching (i.e. If the 'Fetch' button was pressed recently)
        // This is to avoid fetching multiple times in quick succession and causing rate limit exceeded problems
        private static bool FetchingActive { get; set; }

        // Tracks whether or not calculations are currently active
        // TODO What is a 'calculation'? 
        private static bool CalculationActive { get; set; }
        
        private static LogWatcher Watcher { get; set; }

        public bool IsOpen { get; set; }

        public string WarningMessage
        {
            get => _warningMessage;
            set
            {
                _warningMessage = value;
                OnPropertyChanged("WarningMessage");
            }
        }

        public Visibility WarningMessageVisibility
        {
            get => _warningMessageVisibility;
            set
            {
                _warningMessageVisibility = value;
                OnPropertyChanged("WarningMessageVisibility");
            }
        }

        public double ShadowOpacity
        {
            get => _shadowOpacity;
            set
            {
                _shadowOpacity = value;
                OnPropertyChanged("ShadowOpacity");
            }
        }

        public string FullSetsText
        {
            get => _fullSetsText;
            set
            {
                _fullSetsText = value;
                OnPropertyChanged("FullSetsText");
            }
        }

        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set
            {
                _isIndeterminate = value;
                OnPropertyChanged("IsIndeterminate");
            }
        }

        public string OpenStashOverlayButtonContent
        {
            get => _openStashOverlayButtonContent;
            set
            {
                _openStashOverlayButtonContent = value;
                OnPropertyChanged("OpenStashOverlayButtonContent");
            }
        }

        #region Gear Icon Opacity Getters & Setters
        
        public double AmuletsOpacity
        {
            get => _amuletsOpacity;
            set
            {
                _amuletsOpacity = value;
                OnPropertyChanged("AmuletsOpacity");
            }
        }
        
        public double RingsOpacity
        {
            get => _ringsOpacity;
            set
            {
                _ringsOpacity = value;
                OnPropertyChanged("RingsOpacity");
            }
        }
        
        public double BeltsOpacity
        {
            get => _beltsOpacity;
            set
            {
                _beltsOpacity = value;
                OnPropertyChanged("BeltsOpacity");
            }
        }
        
        public double HelmetOpacity
        {
            get => _helmetOpacity;
            set
            {
                _helmetOpacity = value;
                OnPropertyChanged("HelmetOpacity");
            }
        }
        
        public double GlovesOpacity
        {
            get => _glovesOpacity;
            set
            {
                _glovesOpacity = value;
                OnPropertyChanged("GlovesOpacity");
            }
        }

        public double BootsOpacity
        {
            get => _bootsOpacity;
            set
            {
                _bootsOpacity = value;
                OnPropertyChanged("BootsOpacity");
            }
        }
        
        public double ChestsOpacity
        {
            get => _chestsOpacity;
            set
            {
                _chestsOpacity = value;
                OnPropertyChanged("ChestsOpacity");
            }
        }

        public double WeaponsOpacity
        {
            get => _weaponsOpacity;
            set
            {
                _weaponsOpacity = value;
                OnPropertyChanged("WeaponsOpacity");
            }
        }
        
        #endregion

        #region Gear Counter Getters & Setters
        
        public int AmuletsAmount
        {
            get => _amuletsAmount;
            set
            {
                _amuletsAmount = value;
                OnPropertyChanged("AmuletsAmount");
            }
        }
        
        public int RingsAmount
        {
            get => _ringsAmount;
            set
            {
                _ringsAmount = value;
                OnPropertyChanged("RingsAmount");
            }
        }

        public int BeltsAmount
        {
            get => _beltsAmount;
            set
            {
                _beltsAmount = value;
                OnPropertyChanged("BeltsAmount");
            }
        }
        
        public int HelmetsAmount
        {
            get => _helmetsAmount;
            set
            {
                _helmetsAmount = value;
                OnPropertyChanged("HelmetsAmount");
            }
        }

        public int GlovesAmount
        {
            get => _glovesAmount;
            set
            {
                _glovesAmount = value;
                OnPropertyChanged("GlovesAmount");
            }
        }
        
        public int BootsAmount
        {
            get => _bootsAmount;
            set
            {
                _bootsAmount = value;
                OnPropertyChanged("BootsAmount");
            }
        }
        
        public int ChestsAmount
        {
            get => _chestsAmount;
            set
            {
                _chestsAmount = value;
                OnPropertyChanged("ChestsAmount");
            }
        }

        public int WeaponsAmount
        {
            get => _weaponsAmount;
            set
            {
                _weaponsAmount = value;
                OnPropertyChanged("WeaponsAmount");
            }
        }

        #endregion
        
        public Visibility AmountsVisibility
        {
            get => _amountsVisibility;
            set
            {
                _amountsVisibility = value;
                OnPropertyChanged("AmountsVisibility");
            }
        }

        private SolidColorBrush FetchButtonColor
        {
            get => _fetchButtonColor;
            set
            {
                _fetchButtonColor = value;
                OnPropertyChanged("FetchButtonColor");
            }
        }
        
        public bool FetchButtonEnabled
        {
            get => _fetchButtonEnabled;
            set
            {
                _fetchButtonEnabled = value;
                OnPropertyChanged("FetchButtonEnabled");
            }
        }

        public static void DisableWarnings()
        {
            MainWindow.overlay.WarningMessage = "";
            MainWindow.overlay.ShadowOpacity = 0;
            MainWindow.overlay.WarningMessageVisibility = Visibility.Hidden;
        }

        private async void FetchData()
        {
            if (FetchingActive) return;

            if (!Settings.Default.ChaosRecipe && !Settings.Default.RegalRecipe && !Settings.Default.ExaltedRecipe)
            {
                MessageBox.Show("No recipes are enabled. Please pick a recipe.", "No Recipes", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DisableWarnings();
            FetchingActive = true;
            CalculationActive = true;

            Dispatcher.Invoke(() =>
            {
                IsIndeterminate = true;
                FetchButtonEnabled = false;
                FetchButtonColor = Brushes.DimGray;
            });
            await Dispatcher.Invoke(async () =>
            {
                if (await ApiAdapter.GenerateUri())
                    if (await ApiAdapter.GetItems())
                        try
                        {
                            await Task.Run(async () =>
                            {
                                await Data.CheckActives();
                                
                                SetOpacity();
                                CalculationActive = false;
                                Dispatcher.Invoke(() => { IsIndeterminate = false; });
                            }, Data.CancelationToken);

                            await Task.Delay(FetchCooldown * 1000).ContinueWith(_ =>
                            {
                                Trace.WriteLine("waited fetchcooldown");
                                //FetchButtonEnabled = true;
                                //FetchButtonColor = Brushes.Green;
                                //FetchingActive = false;
                            });
                        }
                        catch (OperationCanceledException ex) when (ex.CancellationToken == Data.CancelationToken)
                        {
                            Trace.WriteLine("abort");
                        }

                if (RateLimit.RateLimitExceeded)
                {
                    int secondsToWait = RateLimit.GetSecondsToWait();
                    MainWindow.overlay.WarningMessage = $"Rate Limit Exceeded! Waiting {secondsToWait} seconds...";
                    MainWindow.overlay.ShadowOpacity = 1;
                    MainWindow.overlay.WarningMessageVisibility = Visibility.Visible;
                    
                    await Task.Delay(secondsToWait * 1000);

                    RateLimit.Reset();
                }

                if (RateLimit.BanTime > 0)
                {
                    MainWindow.overlay.WarningMessage = "Temporary Ban! Waiting...";
                    MainWindow.overlay.ShadowOpacity = 1;
                    MainWindow.overlay.WarningMessageVisibility = Visibility.Visible;
                    
                    await Task.Delay(RateLimit.BanTime * 1000);
                    
                    RateLimit.BanTime = 0;
                }
            });
            
            CalculationActive = false;
            FetchingActive = false;
            Dispatcher.Invoke(() =>
            {
                IsIndeterminate = false;
                FetchButtonEnabled = true;
                FetchButtonColor = Brushes.Green;
                FetchingActive = false;
            });
            Trace.WriteLine("end of fetch function reached");
        }
        
        public void RunFetching()
        {
            if (!MainWindow.SettingsComplete) return;
            
            if (!IsOpen) return;
            
            switch (Settings.Default.StashTabMode)
            {
                case 0 when Settings.Default.StashTabIndices == "":
                    MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set Stash Tab Indices.");
                    return;
                case 1 when Settings.Default.StashTabName == "":
                    MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set Stash Tab Prefix.");
                    return;
            }

            if (CalculationActive)
            {
                Data.cs.Cancel();
                FetchingActive = false;
            }
            else
            {
                if (ApiAdapter.IsFetching) return;
                
                Data.cs = new CancellationTokenSource();
                Data.CancelationToken = Data.cs.Token;
                if (MainWindow.stashTabOverlay.IsOpen) MainWindow.stashTabOverlay.Hide();
                FetchData();
                FetchingActive = true;
            }
        }
        
        public void ReloadItemFilter()
        {
            Model.ReloadItemFilter.ReloadFilter();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            FetchData();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !Settings.Default.LockOverlayPosition)
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                    DragMove();
        }

        private void SetOpacity()
        {
            Dispatcher.Invoke(() =>
            {
                if (!Data.ActiveItems.HelmetActive)
                    HelmetOpacity = DeactivatedOpacity;
                else
                    HelmetOpacity = ActivatedOpacity;
                
                if (!Data.ActiveItems.GlovesActive)
                    GlovesOpacity = DeactivatedOpacity;
                else
                    GlovesOpacity = ActivatedOpacity;
                
                if (!Data.ActiveItems.BootsActive)
                    BootsOpacity = DeactivatedOpacity;
                else
                    BootsOpacity = ActivatedOpacity;
                
                if (!Data.ActiveItems.WeaponActive)
                    WeaponsOpacity = DeactivatedOpacity;
                else
                    WeaponsOpacity = ActivatedOpacity;
                
                if (!Data.ActiveItems.ChestActive)
                    ChestsOpacity = DeactivatedOpacity;
                else
                    ChestsOpacity = ActivatedOpacity;
                
                if (!Data.ActiveItems.RingActive)
                    RingsOpacity = DeactivatedOpacity;
                else
                    RingsOpacity = ActivatedOpacity;
                
                if (!Data.ActiveItems.AmuletActive)
                    AmuletsOpacity = DeactivatedOpacity;
                else
                    AmuletsOpacity = ActivatedOpacity;
                
                if (!Data.ActiveItems.BeltActive)
                    BeltsOpacity = DeactivatedOpacity;
                else
                    BeltsOpacity = ActivatedOpacity;
            });
        }

        public new virtual void Hide()
        {
            IsOpen = false;
            if (LogWatcher.WorkerThread != null && LogWatcher.WorkerThread.IsAlive) LogWatcher.StopWatchingLogFile();
            //aTimer.Enabled = false;

            //((MainWindow)System.Windows.Application.Current.MainWindow).RunButtonContent = "Run Overlay";
            base.Hide();
        }

        public new virtual void Show()
        {
            IsOpen = true;
            if (Settings.Default.AutoFetch) Watcher = new LogWatcher();
            //FetchButtonBottomText = "Start";
            //if (FetchingActive)
            //{
            //    aTimer.Enabled = true;
            //    //FetchData();
            //    //FetchButtonBottomText = "Stop";
            //}
            //((MainWindow)System.Windows.Application.Current.MainWindow).RunButtonContent = "Stop Overlay";

            base.Show();
        }


        //private void EditStashTabOverlay_Click(object sender, RoutedEventArgs e)
        //{
        //    if (MainWindow.stashTabOverlay.IsOpen)
        //    {
        //        HandleEditButton();
        //    }
        //    else
        //    {
        //        MainWindow.RunStashTabOverlay();
        //        if (MainWindow.stashTabOverlay.IsOpen)
        //        {
        //            HandleEditButton();
        //        }
        //    }
        //}

        protected override void OnClosing(CancelEventArgs e)
        {
        }

        //private void Window_Deactivated(object sender, EventArgs e)
        //{
        //    // The Window was deactivated 
        //    MainWindow.overlay.Topmost = true;
        //}

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