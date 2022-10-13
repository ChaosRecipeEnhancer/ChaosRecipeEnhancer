using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ChaosRecipeEnhancer.UI.Model.Utils;
using ChaosRecipeEnhancer.UI.Properties;
using IWshRuntimeLibrary;

// REF: https://stackoverflow.com/a/1635680
using HWND = System.IntPtr;

namespace ChaosRecipeEnhancer.UI.Model
{
    public static class ReloadItemFilter
    {
        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(HWND hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(HWND hWnd);

        [DllImport("USER32.DLL")]
        private static extern HWND GetShellWindow();

        [DllImport("USER32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetForegroundWindow(HWND hWnd);

        /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
        /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
        /// REF: https://stackoverflow.com/a/43640787
        private static IDictionary<HWND, string> GetOpenWindows()
        {
            var shellWindow = GetShellWindow();
            var windows = new Dictionary<HWND, string>();

            EnumWindows(delegate(HWND hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                var length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                var builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;
            }, 0);

            return windows;
        }

        public static void ReloadFilter()
        {
            var chatCommand = BuildFilterReloadCommand();
            if (chatCommand is null) return;

            // Map all current window names to their associated "handle to a window" pointers (HWND)
            var openWindows = GetOpenWindows();
            foreach (var window in openWindows)
            {
                var handle = window.Key;
                var title = window.Value;

                Console.WriteLine("{0}: {1}", handle, title);
            }

            // Find the Process ID associated with the 'Path of Exile' game window
            var poeWindow = openWindows.FirstOrDefault(x => x.Value == "Path of Exile").Key;
            if (poeWindow == HWND.Zero)
            {
                UserWarning.WarnUser("Could not find Window! Please make sure Path of Exile is running.",
                    "Window not found");
                return;
            }

            // Get 'Path of Exile' window in the foreground to actually send input to said window
            SetForegroundWindow(poeWindow);

            // Workaround to speed up key input since SendKeys.SendWait() was taking around 5+ seconds (especially with longer filter names)
            // REF: https://social.msdn.microsoft.com/Forums/en-US/3caa1210-e6fd-4f4e-a11c-c8c06e802a6f/sendkeys-too-slow-c-winform?forum=csharpgeneral
            var scriptHost = new WshShell();

            scriptHost.SendKeys("{ENTER}");
            scriptHost.SendKeys(chatCommand);
            scriptHost.SendKeys("{ENTER}");
        }

        private static string BuildFilterReloadCommand()
        {
            var filterName = GetFilterName();
            
            if (!string.IsNullOrEmpty(filterName)) return "/itemfilter " + filterName;

            UserWarning.WarnUser("No filter found. Please set your filter in settings", "No filter found");
            
            return null;
        }

        private static string GetFilterName()
        {
            return Path.GetFileName(Settings.Default.LootFilterFileLocation).Replace(".filter", "");
        }

        private delegate bool EnumWindowsProc(HWND hWnd, int lParam);
    }
}