using System;
using System.Runtime.InteropServices;

namespace ChaosRecipeEnhancer.App.Native
{
    /// <summary>
    ///     Clipboard utility class leveraging existing 32-bit DLL's bundled with Windows
    /// </summary>
    /// <seealso href="https://stackoverflow.com/a/30165665/10072406" />
    public static class ClipboardNative
    {
        private const uint CF_UNICODETEXT = 13;

        [DllImport("user32.dll")]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        private static extern bool SetClipboardData(uint uFormat, IntPtr data);

        public static bool CopyTextToClipboard(string text)
        {
            if (!OpenClipboard(IntPtr.Zero)) return false;

            var global = Marshal.StringToHGlobalUni(text);

            SetClipboardData(CF_UNICODETEXT, global);
            CloseClipboard();

            //-------------------------------------------
            // Not sure, but it looks like we do not need 
            // to free HGLOBAL because Clipboard is now 
            // responsible for the copied data. (?)
            //
            // Otherwise the second call will crash
            // the app with a Win32 exception 
            // inside OpenClipboard() function
            //-------------------------------------------
            // Marshal.FreeHGlobal(global);

            return true;
        }
    }
}