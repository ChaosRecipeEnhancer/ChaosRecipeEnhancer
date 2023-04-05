using System;
using System.Runtime.InteropServices;

namespace ChaosRecipeEnhancer.App.Native
{
    public static class NativeWindowHandler
    {
        private const int WsExTransparent = 0x00000020;
        private const int GwlExStyle = -20;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int index, int newStyle);

        public static void MakeTransparent(IntPtr hWnd)
        {
            // Change the extended window style to include WsExTransparent
            var extendedStyle = GetWindowLong(hWnd, GwlExStyle);
            SetWindowLong(hWnd, GwlExStyle, extendedStyle | WsExTransparent);
        }

        public static void MakeNormal(IntPtr hWnd)
        {
            var extendedStyle = GetWindowLong(hWnd, GwlExStyle);
            SetWindowLong(hWnd, GwlExStyle, extendedStyle & ~WsExTransparent);
        }
    }
}