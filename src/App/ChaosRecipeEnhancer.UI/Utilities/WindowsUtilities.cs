using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace ChaosRecipeEnhancer.UI.Utilities;

internal static class WindowsUtilities
{
    public const int WS_EX_TRANSPARENT = 0x00000020;
    public const int WS_EX_TOOLWINDOW = 0x00000080;
    public const int GWL_EXSTYLE = -20;

    [DllImport("USER32.dll")]
    private static extern int GetWindowLong(nint hWnd, int index);

    [DllImport("USER32.dll")]
    private static extern int SetWindowLong(nint hWnd, int index, int newStyle);

    [DllImport("USER32.DLL")]
    private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

    [DllImport("USER32.DLL")]
    private static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("USER32.DLL")]
    private static extern int GetWindowTextLength(nint hWnd);

    [DllImport("USER32.DLL")]
    private static extern bool IsWindowVisible(nint hWnd);

    [DllImport("USER32.DLL")]
    private static extern nint GetShellWindow();

    [DllImport("USER32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetForegroundWindow(nint hWnd);

    public static void MakeTransparent(Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        _ = SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
    }

    public static void MakeNormal(Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        _ = SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
    }

    // Ensures window does not show up in Alt+Tab or Win+Tab UI
    public static void MakeToolWindow(Window window)
    {
        var hwnd = new WindowInteropHelper(window).Handle;
        int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        _ = SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOOLWINDOW);
    }

    /// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
    /// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
    /// REF: https://stackoverflow.com/a/43640787
    public static IDictionary<nint, string> GetOpenWindows()
    {
        var shellWindow = GetShellWindow();
        var windows = new Dictionary<nint, string>();

        EnumWindows(delegate (nint hWnd, int lParam)
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

    public static bool CheckIfWindowExists(nint window)
    {
        return window == nint.Zero;
    }

    private delegate bool EnumWindowsProc(nint hWnd, int lParam);
}
