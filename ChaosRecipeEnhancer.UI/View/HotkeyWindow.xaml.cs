using System.Windows;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;
using Serilog;

//TODO: Auto-focus on open
namespace ChaosRecipeEnhancer.UI.View
{
    /// <summary>
    /// Interaction logic for HotkeyWindow.xaml
    /// </summary>
    public partial class HotkeyWindow
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly SettingsWindow _settingsWindow;
        private readonly string _type;

        #endregion

        #region Constructors

        public HotkeyWindow(SettingsWindow settingsWindow, string hotkeyType)
        {
            _logger = Log.ForContext<HotkeyWindow>();
            _logger.Debug("Constructing HotkeyWindow");

            _settingsWindow = settingsWindow;
            _type = hotkeyType;
            InitializeComponent();

            _logger.Debug("HotkeyWindow constructed successfully");
        }

        #endregion

        #region Event Handlers

        private void SaveHotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_type)
            {
                case "refresh":
                {
                    HotkeysManager.RemoveHotkey(HotkeysManager.refreshModifier, HotkeysManager.refreshKey);
                    if (CustomHotkeyToggle.Hotkey == null)
                    {
                        Settings.Default.HotkeyRefresh = "< not set >";
                    }
                    else
                    {
                        Settings.Default.HotkeyRefresh = CustomHotkeyToggle.Hotkey.ToString();
                        HotkeysManager.GetRefreshHotkey();
                    }

                    ReApplyHotkeys();
                    break;
                }
                case "toggle":
                {
                    HotkeysManager.RemoveHotkey(HotkeysManager.toggleModifier, HotkeysManager.toggleKey);
                    if (CustomHotkeyToggle.Hotkey == null)
                    {
                        Settings.Default.HotkeyToggle = "< not set >";
                    }
                    else
                    {
                        Settings.Default.HotkeyToggle = CustomHotkeyToggle.Hotkey.ToString();
                        HotkeysManager.GetToggleHotkey();
                    }

                    ReApplyHotkeys();
                    break;
                }
                case "stashtab":
                {
                    HotkeysManager.RemoveHotkey(HotkeysManager.stashTabModifier, HotkeysManager.stashTabKey);
                    if (CustomHotkeyToggle.Hotkey == null)
                    {
                        Settings.Default.HotkeyStashTab = "< not set >";
                    }
                    else
                    {
                        Settings.Default.HotkeyStashTab = CustomHotkeyToggle.Hotkey.ToString();
                        HotkeysManager.GetStashTabHotkey();
                    }

                    ReApplyHotkeys();
                    break;
                }
            }

            Close();
        }

        #endregion

        #region Methods

        private void ReApplyHotkeys()
        {
            _settingsWindow.RemoveAllHotkeys();
            _settingsWindow.AddAllHotkeys();
        }

        #endregion
    }
}