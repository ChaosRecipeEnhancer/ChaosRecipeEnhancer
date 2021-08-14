﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


//Todo: auto focus on open

namespace EnhancePoE
{
    /// <summary>
    /// Interaction logic for HotkeyWindow.xaml
    /// </summary>
    public partial class HotkeyWindow : Window
    {
        private string type;
        private MainWindow mainWindow;

        public HotkeyWindow(MainWindow mainWindow, string hotkeyType)
        {
            this.mainWindow = mainWindow;

            this.type = hotkeyType;

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

            if(type == "refresh")
            {
                HotkeysManager.RemoveHotkey(HotkeysManager.refreshModifier, HotkeysManager.refreshKey);
                if (CustomHotkeyToggle.Hotkey == null)
                {
                    Properties.Settings.Default.HotkeyRefresh = "< not set >";
                }
                else
                {
                    Properties.Settings.Default.HotkeyRefresh = CustomHotkeyToggle.Hotkey.ToString();
                    HotkeysManager.GetRefreshHotkey();
                }
                ReApplyHotkeys();
            }
            else if(type == "toggle")
            {
                HotkeysManager.RemoveHotkey(HotkeysManager.toggleModifier, HotkeysManager.toggleKey);
                if (CustomHotkeyToggle.Hotkey == null)
                {
                    Properties.Settings.Default.HotkeyToggle = "< not set >";
                }
                else
                {
                    Properties.Settings.Default.HotkeyToggle = CustomHotkeyToggle.Hotkey.ToString();
                    HotkeysManager.GetToggleHotkey();
                }
                ReApplyHotkeys();
            }
            else if (type == "stashtab")
            {
                HotkeysManager.RemoveHotkey(HotkeysManager.stashTabModifier, HotkeysManager.stashTabKey);
                if (CustomHotkeyToggle.Hotkey == null)
                {
                    Properties.Settings.Default.HotkeyStashTab = "< not set >";
                }
                else
                {
                    Properties.Settings.Default.HotkeyStashTab = CustomHotkeyToggle.Hotkey.ToString();
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
            this.Close();
        }

        private void ReApplyHotkeys()
        {
            mainWindow.RemoveAllHotkeys();
            mainWindow.AddAllHotkeys();
        }
    }
}
