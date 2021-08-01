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



        public static void ReloadItemFilterFile()
        {
            // store current clipboard
            string oldClipboard = Clipboard.GetText();

            Clipboard.SetText(BuildChatCommand());


            // get Path of Exile in the foreground to actually sendKeys to it
            SetForegroundWindow(FindWindow(null, "Path of Exile"));

            // send the chat commands
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");
            System.Windows.Forms.SendKeys.SendWait("^(v)");
            System.Windows.Forms.SendKeys.SendWait("{ENTER}");

            // restore clipboard
            Clipboard.SetText(oldClipboard);

        }

        public static void ReloadItemFilterOnline()
        {
            //TODO get the name and test how to reload onlinefilter
        }

        private static string BuildChatCommand()
        {
            //build chat command for filterFile
            string filterLocation = Properties.Settings.Default.LootfilterLocation;

            string filterName = Path.GetFileName(filterLocation).Replace(".filter", "");

            return "/itemfilter " + filterName;
        }
    }
}
