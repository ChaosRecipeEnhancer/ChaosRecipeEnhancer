using System;
using System.IO;
using System.Linq;
using ChaosRecipeEnhancer.UI.BusinessLogic.Constants;
using ChaosRecipeEnhancer.UI.DynamicControls;
using ChaosRecipeEnhancer.UI.Extensions.Native;
using ChaosRecipeEnhancer.UI.Properties;
using IWshRuntimeLibrary;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation
{
    public static class ReloadItemFilterHandler
    {
        public static void ReloadFilter()
        {
            var chatCommand = BuildFilterReloadCommand();
            if (chatCommand is null) return;

            // Map all current window names to their associated "handle to a window" pointers (HWND)
            var openWindows = NativeWindowExtensions.GetOpenWindows();
            
            foreach (var window in openWindows)
            {
                var handle = window.Key;
                var title = window.Value;

                Console.WriteLine("{0}: {1}", handle, title);
            }

            // Find the Process ID associated with the 'Path of Exile' game window
            var poeWindow = openWindows.FirstOrDefault(x => x.Value == "Path of Exile").Key;
            
            if (NativeWindowExtensions.CheckIfWindowExists(poeWindow))
            {
                ErrorWindow.Spawn("Could not find PoE window! Please make sure PoE is running." + StringConstruction.DoubleNewLineCharacter +
                                     " If PoE is running in admin mode, try running our app in admin mode, as well.", "Error: PoE Window Not Found");
                return;
            }

            // Get 'Path of Exile' window in the foreground to actually send input to said window
            NativeWindowExtensions.SetForegroundWindow(poeWindow);

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

            ErrorWindow.Spawn("Please configure your filter file location in the settings.", "Error: No Filter File Location Set");
            
            return null;
        }

        private static string GetFilterName()
        {
            return Path.GetFileName(Settings.Default.LootFilterFileLocation).Replace(".filter", "");
        }
    }
}