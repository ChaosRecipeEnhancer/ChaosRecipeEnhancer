using EnhancePoE.Model;
using EnhancePoE.UserControls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace EnhancePoE
{
    /// <summary>
    /// Interaction logic for ChaosRecipeEnhancer.xaml
    /// </summary>
    public partial class ChaosRecipeEnhancer : Window, INotifyPropertyChanged
    {


        // toggle fetch button
        public static bool FetchingActive { get; set;} = false;
        // fetching and calculations currently active
        public static bool CalculationActive { get; set; } = false;
        public static System.Timers.Timer aTimer;

        private static readonly double deactivatedOpacity = .1;
        private static readonly double activatedOpacity = 1;

        public bool IsOpen { get; set; } = false;


        private string _warningMessage;
        public string WarningMessage
        {
            get
            {
                return _warningMessage;
            }
            set
            {
                _warningMessage = value;
                OnPropertyChanged("WarningMessage");
            }
        }

        private Visibility _warningMessageVisibility = Visibility.Hidden;
        public Visibility WarningMessageVisibility
        {
            get
            {
                return _warningMessageVisibility;
            }
            set
            {
                _warningMessageVisibility = value;
                OnPropertyChanged("WarningMessageVisibility");
            }
        }
        private double _shadowOpacity = 0;
        public double ShadowOpacity
        {
            get
            {
                return _shadowOpacity;
            }
            set
            {
                _shadowOpacity = value;
                OnPropertyChanged("ShadowOpacity");
            }
        }

        private string _fullSetsText = "0";
        public string FullSetsText
        {
            get { return _fullSetsText; }
            set
            {
                _fullSetsText = value;
                OnPropertyChanged("FullSetsText");
            }
        }

        private bool _isIndeterminate = false;
        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set
            {
                _isIndeterminate = value;
                OnPropertyChanged("IsIndeterminate");
            }
        }

        private string _fetchButtonBottomText = "Start";
        public string FetchButtonBottomText
        {
            get { return _fetchButtonBottomText; }
            set
            {
                _fetchButtonBottomText = value;
                OnPropertyChanged("FetchButtonBottomText");
            }
        }

        private string _openStashOverlayButtonContent = "Stash";
        public string OpenStashOverlayButtonContent
        {
            get { return _openStashOverlayButtonContent; }
            set
            {
                _openStashOverlayButtonContent = value;
                OnPropertyChanged("OpenStashOverlayButtonContent");
            }
        }

        private double _helmetOpacity = activatedOpacity;
        public double HelmetOpacity
        {
            get { return _helmetOpacity; }
            set
            {
                _helmetOpacity = value;
                OnPropertyChanged("HelmetOpacity");
            }
        }
        private double _bootsOpacity = activatedOpacity;
        public double BootsOpacity
        {
            get { return _bootsOpacity; }
            set
            {
                _bootsOpacity = value;
                OnPropertyChanged("BootsOpacity");
            }
        }        
        private double _glovesOpacity = activatedOpacity;
        public double GlovesOpacity
        {
            get { return _glovesOpacity; }
            set
            {
                _glovesOpacity = value;
                OnPropertyChanged("GlovesOpacity");
            }
        }        
        private double _chestsOpacity = activatedOpacity;
        public double ChestsOpacity
        {
            get { return _chestsOpacity; }
            set
            {
                _chestsOpacity = value;
                OnPropertyChanged("ChestsOpacity");
            }
        }        
        private double _weaponsOpacity = activatedOpacity;
        public double WeaponsOpacity
        {
            get { return _weaponsOpacity; }
            set
            {
                _weaponsOpacity = value;
                OnPropertyChanged("WeaponsOpacity");
            }
        }
        private double _ringsOpacity = activatedOpacity;
        public double RingsOpacity
        {
            get { return _ringsOpacity; }
            set
            {
                _ringsOpacity = value;
                OnPropertyChanged("RingsOpacity");
            }
        }
        private double _amuletsOpacity = activatedOpacity;
        public double AmuletsOpacity
        {
            get { return _amuletsOpacity; }
            set
            {
                _amuletsOpacity = value;
                OnPropertyChanged("AmuletsOpacity");
            }
        }
        private double _beltsOpacity = activatedOpacity;
        public double BeltsOpacity
        {
            get { return _beltsOpacity; }
            set
            {
                _beltsOpacity = value;
                OnPropertyChanged("BeltsOpacity");
            }
        }

        private ContentElement _mainOverlayContent;
        public ContentElement MainOverlayContent
        {
            get { return _mainOverlayContent; }
            set
            {
                _mainOverlayContent = value;
                OnPropertyChanged("MainOverlayContent");
            }
        }

        private SolidColorBrush _fetchButtonColor = Brushes.Red;
        public SolidColorBrush FetchButtonColor
        {
            get { return _fetchButtonColor; }
            set
            {
                _fetchButtonColor = value;
                OnPropertyChanged("FetchButtonColor");
            }
        }

        private int _ringsAmount;
        public int RingsAmount
        {
            get { return _ringsAmount; }
            set
            {
                _ringsAmount = value;
                OnPropertyChanged("RingsAmount");
            }
        }        
        private int _amuletsAmount;
        public int AmuletsAmount
        {
            get { return _amuletsAmount; }
            set
            {
                _amuletsAmount = value;
                OnPropertyChanged("AmuletsAmount");
            }
        }        
        private int _beltsAmount;
        public int BeltsAmount
        {
            get { return _beltsAmount; }
            set
            {
                _beltsAmount = value;
                OnPropertyChanged("BeltsAmount");
            }
        }        
        private int _chestsAmount;
        public int ChestsAmount
        {
            get { return _chestsAmount; }
            set
            {
                _chestsAmount = value;
                OnPropertyChanged("ChestsAmount");
            }
        }        
        private int _weaponsAmount;
        public int WeaponsAmount
        {
            get { return _weaponsAmount; }
            set
            {
                _weaponsAmount = value;
                OnPropertyChanged("WeaponsAmount");
            }
        }        
        private int _glovesAmount;
        public int GlovesAmount
        {
            get { return _glovesAmount; }
            set
            {
                _glovesAmount = value;
                OnPropertyChanged("GlovesAmount");
            }
        }        
        private int _helmetsAmount;
        public int HelmetsAmount
        {
            get { return _helmetsAmount; }
            set
            {
                _helmetsAmount = value;
                OnPropertyChanged("HelmetsAmount");
            }
        }        
        private int _bootsAmount;
        public int BootsAmount
        {
            get { return _bootsAmount; }
            set
            {
                _bootsAmount = value;
                OnPropertyChanged("BootsAmount");
            }
        }

        private Visibility _amountsVisibility = Visibility.Hidden;
        public Visibility AmountsVisibility
        {
            get { return _amountsVisibility; }
            set
            {
                _amountsVisibility = value;
                OnPropertyChanged("AmountsVisibility");
            }
        }


        public static int FullSets { get; set; } = 0;
        public ChaosRecipeEnhancer()
        {
            InitializeComponent();
            DataContext = this;
            aTimer = new System.Timers.Timer();
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = false;
            FullSetsText = "0";
        }


        private async void FetchData()
        {
            MainWindow.overlay.WarningMessage = "";
            MainWindow.overlay.ShadowOpacity = 0;
            MainWindow.overlay.WarningMessageVisibility = System.Windows.Visibility.Hidden;
            //aTimer.Stop();
            CalculationActive = true;
            this.Dispatcher.Invoke(() =>
            {
                IsIndeterminate = true;
            });
            await this.Dispatcher.Invoke(async() =>
            {
                GetFrequency();
                if(await ApiAdapter.GenerateUri())
                {
                    if(await ApiAdapter.GetItems())
                    {
                        try
                        {
                            await Task.Run(() =>
                            {
                                Data.CheckActives();
                                SetOpacity();

                                //ChaosRecipeEnhancer.aTimer.Enabled = true;
                                Trace.WriteLine("timer enabled");
                                aTimer.Enabled = true;
                                CalculationActive = false;
                                this.Dispatcher.Invoke(() =>
                                {
                                    IsIndeterminate = false;

                                });
                            }, Data.ct);
                        }
                        catch (OperationCanceledException ex) when (ex.CancellationToken == Data.ct)
                        {
                            Trace.WriteLine("abort");
                            CalculationActive = false;
                            this.Dispatcher.Invoke(() =>
                            {
                                IsIndeterminate = false;

                            });

                        }
                    }
                }
                if (RateLimit.RateLimitExceeded)
                {
                    // TODO: show rate limit exceeded in ui
                    MainWindow.overlay.WarningMessage = "Rate Limit Exceeded! Waiting...";
                    MainWindow.overlay.ShadowOpacity = 1;
                    MainWindow.overlay.WarningMessageVisibility = System.Windows.Visibility.Visible;
                    aTimer.Enabled = false;
                    await Task.Delay(RateLimit.GetSecondsToWait() * 1000);
                    RateLimit.RequestCounter = 0;
                    RateLimit.RateLimitExceeded = false;
                    aTimer.Enabled = true;
                }
                if (RateLimit.BanTime > 0)
                {
                    MainWindow.overlay.WarningMessage = "Temporary Ban! Waiting...";
                    MainWindow.overlay.ShadowOpacity = 1;
                    MainWindow.overlay.WarningMessageVisibility = System.Windows.Visibility.Visible;
                    aTimer.Enabled = false;
                    await Task.Delay(RateLimit.BanTime * 1000);
                    RateLimit.BanTime = 0;
                    aTimer.Enabled = true;
                }

            });
        }


        public void RunFetching()
        {
            if (MainWindow.SettingsComplete)
            {
                if (!IsOpen)
                {
                    return;
                }
                if (Properties.Settings.Default.StashtabMode == 0)
                {
                    if (Properties.Settings.Default.StashTabIndices == "")
                    {
                        MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set Stashtab Indices.");
                        return;
                    }
                }
                else if (Properties.Settings.Default.StashtabMode == 1)
                {
                    if (Properties.Settings.Default.StashTabName == "")
                    {
                        MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set Stashtab Prefix.");
                        return;
                    }
                }
                if (CalculationActive || aTimer.Enabled)
                {
                    Data.cs.Cancel();
                    aTimer.Enabled = false;
                    FetchingActive = false;
                    //RefreshButton.Content = "Fetch\nStart";
                    FetchButtonBottomText = "Start";
                    FetchButtonColor = Brushes.Red;
                }
                else
                {
                    if (!ApiAdapter.IsFetching)
                    {
                        Data.cs = new System.Threading.CancellationTokenSource();
                        Data.ct = Data.cs.Token;
                        if (MainWindow.stashTabOverlay.IsOpen)
                        {
                            MainWindow.stashTabOverlay.Hide();
                        }
                        FetchData();
                        //aTimer.Interval = 1000;
                        //aTimer.Enabled = true;
                        FetchingActive = true;
                        //RefreshButton.Content = "Stop";
                        FetchButtonBottomText = "Stop";
                        FetchButtonColor = Brushes.Green;
                    }

                }
            }
        }



        // TODO: find better algo for getting frequency, maybe implementing response header thresholds
        private static void GetFrequency()
        {
            int addedTime = 0;
            //if(Properties.Settings.Default.StashTabIndices != "")
            //{
            //    addedTime += (Properties.Settings.Default.StashTabIndices.Length / 2) * 1000;
            //}
            aTimer.Interval = (Properties.Settings.Default.RefreshRate * 1000) + addedTime;
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            FetchData();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void SetOpacity()
        {

            Dispatcher.Invoke(() =>
            {
                if (!Data.ActiveItems.HelmetActive)
                {
                    HelmetOpacity = deactivatedOpacity;
                }
                else
                {
                    HelmetOpacity = activatedOpacity;
                }
                if (!Data.ActiveItems.GlovesActive)
                {
                    GlovesOpacity = deactivatedOpacity;
                }
                else
                {
                    GlovesOpacity = activatedOpacity;
                }
                if (!Data.ActiveItems.BootsActive)
                {
                    BootsOpacity = deactivatedOpacity;
                }
                else
                {
                    BootsOpacity = activatedOpacity;
                }
                if (!Data.ActiveItems.WeaponActive)
                {
                    WeaponsOpacity = deactivatedOpacity;
                }
                else
                {
                    WeaponsOpacity = activatedOpacity;
                }
                if (!Data.ActiveItems.ChestActive)
                {
                    ChestsOpacity = deactivatedOpacity;
                }
                else
                {
                    ChestsOpacity = activatedOpacity;
                }
                if (!Data.ActiveItems.RingActive)
                {
                    RingsOpacity = deactivatedOpacity;
                }
                else
                {
                    RingsOpacity = activatedOpacity;
                }
                if (!Data.ActiveItems.AmuletActive)
                {
                    AmuletsOpacity = deactivatedOpacity;
                }
                else
                {
                    AmuletsOpacity = activatedOpacity;
                }
                if (!Data.ActiveItems.BeltActive)
                {
                    BeltsOpacity = deactivatedOpacity;
                }
                else
                {
                    BeltsOpacity = activatedOpacity;
                }

            });
        }

        public new virtual void Hide()
        {
            IsOpen = false;

            aTimer.Enabled = false;
            
            //((MainWindow)System.Windows.Application.Current.MainWindow).RunButtonContent = "Run Overlay";
            base.Hide();
        }

        public new virtual void Show()
        {
            IsOpen = true;
            FetchButtonBottomText = "Start";
            if (FetchingActive)
            {
                aTimer.Enabled = true;
                //FetchData();
                FetchButtonBottomText = "Stop";
            }
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
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


    }
}
