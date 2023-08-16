using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace ChaosRecipeEnhancer.UI.Utilities;

internal sealed class MouseHookEventArgs : EventArgs
{
	public Point ClickLocation
	{
		get;
	}

	public MouseHookEventArgs(int x, int y) => ClickLocation = new Point(x, y);
}

internal static class MouseHook
{
	public static event EventHandler<MouseHookEventArgs> MouseAction = delegate { };

	public static void Start() => _hookID = SetHook(_proc);
	public static void Stop() => UnhookWindowsHookEx(_hookID);

	public static int ClickLocationX
	{
		get; set;
	}
	public static int ClickLocationY
	{
		get; set;
	}

	private static readonly LowLevelMouseProc _proc = HookCallback;
	private static IntPtr _hookID = IntPtr.Zero;

	private static IntPtr SetHook(LowLevelMouseProc proc)
	{
		var hook = SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle("user32"), 0);
		return hook == IntPtr.Zero ? throw new System.ComponentModel.Win32Exception() : hook;
	}

	private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

	private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
	{
		if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
		{
			var hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

			MouseAction(null, new MouseHookEventArgs(hookStruct.pt.x, hookStruct.pt.y));
		}
		return CallNextHookEx(_hookID, nCode, wParam, lParam);
	}

	private const int WH_MOUSE_LL = 14;

	private enum MouseMessages
	{
		WM_LBUTTONDOWN = 0x0201,
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct POINT
	{
		public int x;
		public int y;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct MSLLHOOKSTRUCT
	{
		public POINT pt;
		public uint mouseData, flags, time;
		public IntPtr dwExtraInfo;
	}

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool UnhookWindowsHookEx(IntPtr hhk);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

	[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr GetModuleHandle(string lpModuleName);
}
