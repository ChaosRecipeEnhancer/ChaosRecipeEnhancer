using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ChaosRecipeEnhancer.UI.Extensions.Native;

public static class NativeWindowExtensions
{
	private const int WsExTransparent = 0x00000020;
	private const int GwlExStyle = -20;

	[DllImport("USER32.dll")]
	private static extern int GetWindowLong(IntPtr hWnd, int index);

	[DllImport("USER32.dll")]
	private static extern int SetWindowLong(IntPtr hWnd, int index, int newStyle);

	[DllImport("USER32.DLL")]
	private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

	[DllImport("USER32.DLL")]
	private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	[DllImport("USER32.DLL")]
	private static extern int GetWindowTextLength(IntPtr hWnd);

	[DllImport("USER32.DLL")]
	private static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("USER32.DLL")]
	private static extern IntPtr GetShellWindow();

	[DllImport("USER32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
	public static extern bool SetForegroundWindow(IntPtr hWnd);

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

	/// <summary>Returns a dictionary that contains the handle and title of all the open windows.</summary>
	/// <returns>A dictionary that contains the handle and title of all the open windows.</returns>
	/// REF: https://stackoverflow.com/a/43640787
	public static IDictionary<IntPtr, string> GetOpenWindows()
	{
		var shellWindow = GetShellWindow();
		var windows = new Dictionary<IntPtr, string>();

		EnumWindows(delegate (IntPtr hWnd, int lParam)
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

	public static bool CheckIfWindowExists(IntPtr window)
	{
		return window == IntPtr.Zero;
	}

	private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
}