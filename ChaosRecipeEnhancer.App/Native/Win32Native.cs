using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ChaosRecipeEnhancer.App.Native
{
    public static class Win32Native
    {
        //Rest of this code in located in the class itself
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = -20;

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("Kernel32.dll")]
        public static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);
        
        public static void makeTransparent(IntPtr hwnd)
        {
            // Change the extended window style to include WS_EX_TRANSPARENT
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);

            //MainWindow.stashTabOverlay.IsHitTestVisible = false;
        }

        public static void makeNormal(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);

            //MainWindow.stashTabOverlay.IsHitTestVisible = true;
        }
    }
}