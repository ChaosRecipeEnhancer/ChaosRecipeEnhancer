﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ChaosRecipeEnhancer.App.Native
{
    // public static class OldMouseHook
    // {
    //     private const int WH_MOUSE_LL = 14;
    //     private static readonly LowLevelMouseProc _proc = HookCallback;
    //     private static IntPtr _hookID = IntPtr.Zero;
    //
    //     public static int ClickLocationX { get; set; }
    //     public static int ClickLocationY { get; set; }
    //     public static event EventHandler MouseAction = delegate { };
    //
    //     public static void Start()
    //     {
    //         _hookID = SetHook(_proc);
    //     }
    //
    //     public static void Stop()
    //     {
    //         UnhookWindowsHookEx(_hookID);
    //     }
    //
    //     private static IntPtr SetHook(LowLevelMouseProc proc)
    //     {
    //         var hook = SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle("user32"), 0);
    //         if (hook == IntPtr.Zero) throw new Win32Exception();
    //         return hook;
    //     }
    //
    //     private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    //     {
    //         if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
    //         {
    //             var hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
    //             //Trace.WriteLine(hookStruct.pt.x + "x");
    //             ClickLocationX = hookStruct.pt.x;
    //             ClickLocationY = hookStruct.pt.y;
    //
    //             MouseAction(null, new EventArgs());
    //         }
    //
    //         return CallNextHookEx(_hookID, nCode, wParam, lParam);
    //     }
    //
    //     [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //     private static extern IntPtr SetWindowsHookEx(int idHook,
    //         LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
    //
    //     [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //     [return: MarshalAs(UnmanagedType.Bool)]
    //     private static extern bool UnhookWindowsHookEx(IntPtr hhk);
    //
    //     [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //     private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
    //         IntPtr wParam, IntPtr lParam);
    //
    //     [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //     private static extern IntPtr GetModuleHandle(string lpModuleName);
    //
    //     private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
    //
    //     private enum MouseMessages
    //     {
    //         WM_LBUTTONDOWN = 0x0201,
    //         WM_LBUTTONUP = 0x0202,
    //         WM_MOUSEMOVE = 0x0200,
    //         WM_MOUSEWHEEL = 0x020A,
    //         WM_RBUTTONDOWN = 0x0204,
    //         WM_RBUTTONUP = 0x0205
    //     }
    //
    //     [StructLayout(LayoutKind.Sequential)]
    //     private struct POINT
    //     {
    //         public readonly int x;
    //         public readonly int y;
    //     }
    //
    //     [StructLayout(LayoutKind.Sequential)]
    //     private struct MSLLHOOKSTRUCT
    //     {
    //         public readonly POINT pt;
    //         public readonly uint mouseData;
    //         public readonly uint flags;
    //         public readonly uint time;
    //         public readonly IntPtr dwExtraInfo;
    //     }
    // }
}