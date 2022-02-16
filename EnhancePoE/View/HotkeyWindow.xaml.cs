using System.Windows;
using EnhancePoE.Properties;

//TODO: Auto-focus on open
namespace EnhancePoE
{
    /// <summary>
    ///     Interaction logic for HotkeyWindow.xaml
    /// </summary>
    public partial class HotkeyWindow
    {
        private readonly MainWindow _mainWindow;
        private readonly string _type;

        public HotkeyWindow(MainWindow mainWindow, string hotkeyType)
        {
            _mainWindow = mainWindow;
            _type = hotkeyType;
            InitializeComponent();
        }

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

        private void ReApplyHotkeys()
        {
            _mainWindow.RemoveAllHotkeys();
            _mainWindow.AddAllHotkeys();
        }
    }
}