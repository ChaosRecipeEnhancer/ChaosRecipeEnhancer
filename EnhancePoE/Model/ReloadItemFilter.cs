using EnhancePoE.Model.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace EnhancePoE.Model
{
    public static class ReloadItemFilter
    {


        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // When you don't want the ProcessId, use this overload and pass IntPtr.Zero for the second parameter
        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);



        public static void ReloadItemfilter()
        {
            // store current clipboard
            string oldClipboard = Clipboard.GetText();

            string chatCommand = BuildChatCommand();
            if(chatCommand is null)
            {
                return;
            }

            Clipboard.SetDataObject(BuildChatCommand());

            var poeWindow = FindWindow(null, "Path of Exile");
            if(poeWindow == IntPtr.Zero)
            {
                UserWarning.WarnUser("Could not find Window! Please make sure Path of Exile is running.", "Window not found");
                return;
            }
            // get Path of Exile in the foreground to actually sendKeys to it
            SetForegroundWindow(poeWindow);

            // send the chat commands
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            System.Windows.Forms.SendKeys.SendWait("^(v)");
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");

            // restore clipboard
            Clipboard.SetDataObject(oldClipboard);

        }

        private static string BuildChatCommand()
        {

            string filterName = GetFilterName().Trim();
            Trace.WriteLine("filtername", filterName);
            if(String.IsNullOrEmpty(filterName))
            {
                UserWarning.WarnUser("No filter found. Please set your filter in settings", "No filter found");
                return null;
            }
            return "/itemfilter " + filterName;
        }

        private static string GetFilterName()
        {
            if (Properties.Settings.Default.LootfilterOnline)
            {
                return Properties.Settings.Default.LootfilterOnlineName.Trim();
            }
            return System.IO.Path.GetFileName(Properties.Settings.Default.LootfilterLocation).Replace(".filter", "");
        }
    }
}
