using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ChaosRecipeEnhancer.UI.WPF.Utilities;

internal static class Win32
{
	public const int WS_EX_TRANSPARENT = 0x00000020;
	public const int WS_EX_TOOLWINDOW = 0x00000080;
	public const int GWL_EXSTYLE = -20;

	[DllImport("user32.dll")]
	public static extern int GetWindowLong(IntPtr hwnd, int index);

	[DllImport("user32.dll")]
	public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

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
}
