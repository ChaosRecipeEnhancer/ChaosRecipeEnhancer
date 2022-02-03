using System.Windows;
using EnhancePoE.Properties;


//Todo: auto focus on open

namespace EnhancePoE
{
    /// <summary>
    ///     Interaction logic for HotkeyWindow.xaml
    /// </summary>
    public partial class HotkeyWindow : Window
    {
        private readonly MainWindow mainWindow;
        private readonly string type;

        public HotkeyWindow(MainWindow mainWindow, string hotkeyType)
        {
            this.mainWindow = mainWindow;

            type = hotkeyType;

            //if(hotkeyType == "refresh")
            //{
            //    type = "refresh";
            //}
            //else if(hotkeyType == "toggle")
            //{
            //    type = "toggle";
            //} 
            //else if(hotkeyType == "stashtab")
            //{
            //    type = "stashtab";
            //}
            //else if(hotkeyType == "reloadFilter")
            //{
            //    type = "reloadFilter";
            //}

            InitializeComponent();
        }

        private void SaveHotkeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (type == "refresh")
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
            }
            else if (type == "toggle")
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
            }
            else if (type == "stashtab")
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
            }
            //else if(type == "reloadFilter")
            //{
            //    HotkeysManager.RemoveHotkey(HotkeysManager.reloadFilterModifier, HotkeysManager.reloadFilterKey);
            //    if (CustomHotkeyToggle.Hotkey == null)
            //    {
            //        Properties.Settings.Default.HotkeyReloadFilter = "< not set >";
            //    }
            //    else
            //    {
            //        Properties.Settings.Default.HotkeyReloadFilter = CustomHotkeyToggle.Hotkey.ToString();
            //        HotkeysManager.GetReloadFilterHotkey();
            //    }
            //    ReApplyHotkeys();

            //}
            Close();
        }

        private void ReApplyHotkeys()
        {
            mainWindow.RemoveAllHotkeys();
            mainWindow.AddAllHotkeys();
        }
    }
}