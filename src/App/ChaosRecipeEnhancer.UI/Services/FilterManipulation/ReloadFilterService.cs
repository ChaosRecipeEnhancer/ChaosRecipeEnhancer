using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Extensions.Native;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Windows;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation;

public interface IReloadFilterService
{
    public void ReloadFilter();
}

public class ReloadFilterService : IReloadFilterService
{
    public void ReloadFilter()
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
            ErrorWindow.Spawn(
                "Could not find PoE window! Please make sure PoE is running.\n\n" +
                " If PoE is running in admin mode, try running our app in admin mode, as well.",
                "Error: Reload Filter - PoE Window Not Found"
            );

            return;
        }

        // Get 'Path of Exile' window in the foreground to actually send input to said window
        NativeWindowExtensions.SetForegroundWindow(poeWindow);

        // Always clear the clipboard first
        Clipboard.Clear();
        Clipboard.SetText(chatCommand);

        SendKeys.SendWait("{ENTER}");
        SendKeys.SendWait("^(v)");
        SendKeys.SendWait("{ENTER}");
    }

    private string BuildFilterReloadCommand()
    {
        var filterName = GetFilterName();

        if (!string.IsNullOrEmpty(filterName)) return "/itemfilter " + filterName;

        ErrorWindow.Spawn(
            "Please configure your filter file location in the settings.",
            "Error: Reload Filter - No Filter File Location Set"
            );

        return null;
    }

    private string GetFilterName()
    {
        return Path.GetFileName(Settings.Default.LootFilterFileLocation)!.Replace(".filter", "");
    }
}