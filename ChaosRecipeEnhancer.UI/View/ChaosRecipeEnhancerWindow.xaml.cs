using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;
using Serilog;

namespace ChaosRecipeEnhancer.UI.View
{
    /// <summary>
    /// Interaction logic for ChaosRecipeEnhancer.xaml
    /// </summary>
    public partial class ChaosRecipeEnhancerWindow : INotifyPropertyChanged
    {
        #region Fields

        private readonly ILogger _logger;

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
        private Visibility _warningMessageVisibility = Visibility.Collapsed;

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

        // Tracks whether or not we're currently fetching (i.e. If the 'Fetch' button was pressed recently)
        // This is to avoid fetching multiple times in quick succession and causing rate limit exceeded problems
        private static bool FetchingActive { get; set; }

        // Tracks whether or not calculations are currently active
        // TODO What is a 'calculation'?
        private static bool CalculationActive { get; set; }

        private static LogWatcher Watcher { get; set; }

        #endregion

        #region Constructors

        public ChaosRecipeEnhancerWindow()
        {
            _logger = Log.ForContext<ChaosRecipeEnhancerWindow>();
            _logger.Debug("Constructing ChaosRecipeEnhancer");

            InitializeComponent();

            DataContext = this;
            FullSetsText = "0";

            _logger.Debug("ChaosRecipeEnhancer constructed successfully");
        }

        #endregion

        #region Properties

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

        #endregion

        #region Event Handlers

        public new virtual void Hide()
        {
            IsOpen = false;
            if (LogWatcher.WorkerThread != null && LogWatcher.WorkerThread.IsAlive) LogWatcher.StopWatchingLogFile();
            base.Hide();
        }

        public new virtual void Show()
        {
            IsOpen = true;
            if (Settings.Default.AutoFetchOnRezoneEnabled) Watcher = new LogWatcher(this);
            base.Show();
        }

        /// <summary>
        /// Handler for a 'Mouse Down' event on our {ChaosRecipeEnhancer} to move the window around.
        /// </summary>
        /// <param name="sender">MouseDown event that triggers our method</param>
        /// <param name="e">Arguments related to the event that we can access when handling it</param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // If the user has locked the {ChaosRecipeEnhancer} in their settings, we ignore the event
            if (e.ChangedButton != MouseButton.Left || Settings.Default.SetTrackerOverlayOverlayLockPositionEnabled) return;

            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                // Native method to move a Window object on the screen
                DragMove();
            }
        }

        #endregion

        #region Methods

        public static void DisableWarnings(ChaosRecipeEnhancerWindow chaosRecipeEnhancerWindow)
        {
            chaosRecipeEnhancerWindow.WarningMessage = "";
            chaosRecipeEnhancerWindow.ShadowOpacity = 0;
            chaosRecipeEnhancerWindow.WarningMessageVisibility = Visibility.Hidden;
        }

        private async void FetchData()
        {
            if (FetchingActive) return;

            if (!Settings.Default.ChaosRecipeTrackingEnabled && !Settings.Default.RegalRecipeTrackingEnabled && !Settings.Default.ExaltedShardRecipeTrackingEnabled)
            {
                MessageBox.Show("No recipes are enabled. Please pick a recipe.", "No Recipes", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            // TODO: [Validate] Find some other way to ensure warnings are disabled on the ChaosRecipeEnhancerOverlay
            DisableWarnings(this);

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
                                await Data.CheckActives(this);

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

                    // TODO: [Refactor] Remove dependency on MainWindow for displaying these warning messages
                    // MainWindow.Overlay.WarningMessage = $"Rate Limit Exceeded! Waiting {secondsToWait} seconds...";
                    // MainWindow.Overlay.ShadowOpacity = 1;
                    // MainWindow.Overlay.WarningMessageVisibility = Visibility.Visible;

                    await Task.Delay(secondsToWait * 1000);

                    RateLimit.Reset();
                }

                if (RateLimit.BanTime > 0)
                {
                    // TODO: [Refactor] Remove dependency on MainWindow for displaying these warning messages
                    // MainWindow.Overlay.WarningMessage = "Temporary Ban! Waiting...";
                    // MainWindow.Overlay.ShadowOpacity = 1;
                    // MainWindow.Overlay.WarningMessageVisibility = Visibility.Visible;

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

            switch (Settings.Default.StashTabQueryMode)
            {
                case 0 when Settings.Default.StashTabIndices == "":
                    MessageBox.Show("Missing Stash Query Settings!" + Environment.NewLine + "Please set stash tab indices.");
                    return;
                case 1 when Settings.Default.StashTabPrefix == "":
                    MessageBox.Show("Missing Stash Query Settings!" + Environment.NewLine + "Please set stash tab prefix.");
                    return;
                case 2 when Settings.Default.StashTabSuffix == "":
                    MessageBox.Show("Missing Stash Query Settings!" + Environment.NewLine + "Please set stash tab suffix.");
                    return;
                
                // TODO: [Refactor] Query by folder name stuff (doesn't work; not supported by API)
                // case 3 when Settings.Default.StashFolderName == "":
                //     MessageBox.Show("Missing Stash Query Settings!" + Environment.NewLine + "Please set stash tab folder name.");
                //     return;
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

                // TODO: [Refactor] Remove dependency on stashTabOverlayView and make sure we hide BEFORE or AFTER calling this (?)
                // if (_stashTabOverlayView.IsOpen) _stashTabOverlayView.Hide();

                FetchData();
                FetchingActive = true;
            }
        }

        public void ReloadItemFilter()
        {
            Model.ReloadItemFilter.ReloadFilter();
        }

        private void SetOpacity()
        {
            Trace.Write("Setting new item opacity");

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

        #endregion
    }
}