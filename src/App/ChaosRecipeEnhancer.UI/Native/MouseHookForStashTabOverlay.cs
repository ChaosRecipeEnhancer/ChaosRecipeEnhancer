using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace ChaosRecipeEnhancer.UI.Native;

public static class MouseHookForStashTabOverlay
{
    private const int WH_MOUSE_LL = 14;

    private static readonly LowLevelMouseProc MouseProcess = HookCallback;
    private static nint _hookID = nint.Zero;

    public static event EventHandler<MouseHookEventArgs> MouseAction = delegate { };

    public static void Start() => _hookID = SetHook(MouseProcess);
    public static void Stop() => UnhookWindowsHookEx(_hookID);

    private static nint SetHook(LowLevelMouseProc proc)
    {
        var hook = SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle("user32"), 0);
        return hook == nint.Zero ? throw new System.ComponentModel.Win32Exception() : hook;
    }

    private delegate nint LowLevelMouseProc(int nCode, nint wParam, nint lParam);

    private static nint HookCallback(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= 0 && (MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam || MouseMessages.WM_LBUTTONUP == (MouseMessages)wParam))
        {
            var hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            var mouseArgs = new MouseHookEventArgs(hookStruct.pt.x, hookStruct.pt.y, (MouseMessages)wParam);

            // Dispatch the MouseAction event to the main UI thread
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                MouseAction(null, mouseArgs);
            }));
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    public enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
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
        public nint dwExtraInfo;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, nint hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint GetModuleHandle(string lpModuleName);
}

public class MouseHookEventArgs : EventArgs
{
    public int X { get; }
    public int Y { get; }
    public MouseHookForStashTabOverlay.MouseMessages Message { get; }

    public MouseHookEventArgs(int x, int y, MouseHookForStashTabOverlay.MouseMessages message)
    {
        X = x;
        Y = y;
        Message = message;
    }
}