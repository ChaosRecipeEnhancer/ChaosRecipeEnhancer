using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ChaosRecipeEnhancer.UI.Utilities.Native;

public static class MouseHookForGeneralInteractionInStashTabOverlay
{
    private const int WhMouseLl = 14;

    private static readonly LowLevelMouseProc MouseProcess = HookCallback;
    private static nint _hookId = nint.Zero;

    public static int ClickLocationX
    {
        get; set;
    }
    public static int ClickLocationY
    {
        get; set;
    }

    public static event EventHandler MouseAction = delegate { };

    public static void Start()
    {
        _hookId = SetHook(MouseProcess);
    }

    public static void Stop()
    {
        UnhookWindowsHookEx(_hookId);
    }

    private static nint SetHook(LowLevelMouseProc proc)
    {
        var hook = SetWindowsHookEx(WhMouseLl, proc, GetModuleHandle("user32"), 0);
        if (hook == nint.Zero) throw new Win32Exception();

        return hook;
    }

    private static nint HookCallback(int nCode, nint wParam, nint lParam)
    {
        if (nCode < 0 || MouseMessages.WM_LBUTTONDOWN != (MouseMessages)wParam)
            return CallNextHookEx(_hookId, nCode, wParam, lParam);

        var hookStruct = (MicrosoftLowLevelHookStruct)Marshal.PtrToStructure(lParam, typeof(MicrosoftLowLevelHookStruct));

        ClickLocationX = hookStruct.pt.x;
        ClickLocationY = hookStruct.pt.y;

        MouseAction(null, EventArgs.Empty);

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
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

    private delegate nint LowLevelMouseProc(int nCode, nint wParam, nint lParam);

    private enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public readonly int x;
        public readonly int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MicrosoftLowLevelHookStruct
    {
        public readonly Point pt;
        private readonly uint mouseData;
        private readonly uint flags;
        private readonly uint time;
        private readonly nint dwExtraInfo;
    }
}