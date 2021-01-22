using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EnhancePoE.Model
{
    static class ForegroundWindows
    {

        //// The GetForegroundWindow function returns a handle to the foreground window
        //// (the window  with which the user is currently working).
        //[System.Runtime.InteropServices.DllImport("user32.dll")]
        //private static extern IntPtr GetForegroundWindow();

        //// The GetWindowThreadProcessId function retrieves the identifier of the thread
        //// that created the specified window and, optionally, the identifier of the
        //// process that created the window.
        //[System.Runtime.InteropServices.DllImport("user32.dll")]
        //private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        //// Returns the name of the process owning the foreground window.
        //public static string GetForegroundProcessName()
        //{
        //    IntPtr hwnd = GetForegroundWindow();

        //    // The foreground window can be NULL in certain circumstances, 
        //    // such as when a window is losing activation.
        //    if (hwnd == null)
        //        return "Unknown";

        //    uint pid;
        //    GetWindowThreadProcessId(hwnd, out pid);

        //    foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
        //    {
        //        if (p.Id == pid)
        //            return p.ProcessName;
        //    }

        //    return "Unknown";
        //}
    }
}
