using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using WindowsInput.Native;
using WindowsInput;

namespace EnhancePoE.Model
{
    public static class SendInputs
    {
        private static string enter = "{Enter}";
        private static string insert = "^v";

        private static InputSimulator sim = new InputSimulator();

        public static void SendInsert(string filterName)
        {
            //string insertText = "/itemfilter " + filterName;            
            string insertText = "test test test";
            sim.Keyboard.TextEntry("Hello Worl !");
            sim.Keyboard.Sleep(1000);

            //Clipboard.Clear();
            //Clipboard.SetDataObject(insertText);
            //SendKeys.SendWait("{Enter}");
            //System.Threading.Thread.Sleep(100);
            //SendKeys.SendWait("Test");
            //SendKeys.SendWait("{Enter}");

            //SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
            //press();
        }

        //void ShowDesktop()
        //{
        //    OutputString(L"Sending 'Win-D'\r\n");
        //    INPUT inputs[4] = { };
        //    ZeroMemory(inputs, sizeof(inputs));

        //    inputs[0].type = INPUT_KEYBOARD;
        //    inputs[0].ki.wVk = VK_LWIN;

        //    inputs[1].type = INPUT_KEYBOARD;
        //    inputs[1].ki.wVk = VK_D;

        //    inputs[2].type = INPUT_KEYBOARD;
        //    inputs[2].ki.wVk = VK_D;
        //    inputs[2].ki.dwFlags = KEYEVENTF_KEYUP;

        //    inputs[3].type = INPUT_KEYBOARD;
        //    inputs[3].ki.wVk = VK_LWIN;
        //    inputs[3].ki.dwFlags = KEYEVENTF_KEYUP;

        //    UINT uSent = SendInput(ARRAYSIZE(inputs), inputs, sizeof(INPUT));
        //    if (uSent != ARRAYSIZE(inputs))
        //    {
        //        OutputString(L"SendInput failed: 0x%x\n", HRESULT_FROM_WIN32(GetLastError()));
        //    }
        //}

        //[DllImport("user32.dll")]
        //public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        //const int VK_UP = 0x26; //up key
        //const int VK_DOWN = 0x28;  //down key
        //const int VK_LEFT = 0x25;
        //const int VK_RIGHT = 0x27;
        //const uint KEYEVENTF_KEYUP = 0x0002;
        //const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        //const int VK_M = 0x4D;

        //static int press()
        //{
        //    //Press the key
        //    keybd_event((byte)VK_M, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        //    keybd_event((byte)VK_M, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        //    return 0;
        //}

        //    [StructLayout(LayoutKind.Sequential)]
        //    public struct KeyboardInput
        //    {
        //        public ushort wVk;
        //        public ushort wScan;
        //        public uint dwFlags;
        //        public uint time;
        //        public IntPtr dwExtraInfo;
        //    }

        //    [StructLayout(LayoutKind.Sequential)]
        //    public struct MouseInput
        //    {
        //        public int dx;
        //        public int dy;
        //        public uint mouseData;
        //        public uint dwFlags;
        //        public uint time;
        //        public IntPtr dwExtraInfo;
        //    }

        //    [StructLayout(LayoutKind.Sequential)]
        //    public struct HardwareInput
        //    {
        //        public uint uMsg;
        //        public ushort wParamL;
        //        public ushort wParamH;
        //    }

        //    [StructLayout(LayoutKind.Explicit)]
        //    public struct InputUnion
        //    {
        //        [FieldOffset(0)] public MouseInput mi;
        //        [FieldOffset(0)] public KeyboardInput ki;
        //        [FieldOffset(0)] public HardwareInput hi;
        //    }

        //    public struct Input
        //    {
        //        public int type;
        //        public InputUnion u;
        //    }

        //    [Flags]
        //    public enum InputType
        //    {
        //        Mouse = 0,
        //        Keyboard = 1,
        //        Hardware = 2
        //    }

        //    [Flags]
        //    public enum KeyEventF
        //    {
        //        KeyDown = 0x0000,
        //        ExtendedKey = 0x0001,
        //        KeyUp = 0x0002,
        //        Unicode = 0x0004,
        //        Scancode = 0x0008
        //    }

        //    [DllImport("user32.dll", SetLastError = true)]
        //    private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        //    [DllImport("user32.dll")]
        //    private static extern IntPtr GetMessageExtraInfo();

        //    static Input[] inputs = new Input[]
        //    {
        //        new Input
        //        {
        //            type = (int)InputType.Keyboard,
        //            u = new InputUnion
        //            {
        //                ki = new KeyboardInput
        //                {
        //                    wVk = 0,
        //                    wScan = 0x0D, // W
        //                    dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
        //                    dwExtraInfo = GetMessageExtraInfo(),
        //                },                    
        //            }
        //        },               
        //        new Input
        //        {
        //            type = (int)InputType.Keyboard,
        //            u = new InputUnion
        //            {
        //                ki = new KeyboardInput
        //                {
        //                    wVk = 0,
        //                    wScan = 0x11, // W
        //                    dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
        //                    dwExtraInfo = GetMessageExtraInfo(),
        //                },
        //            }
        //        },
        //        new Input
        //        {
        //            type = (int)InputType.Keyboard,
        //            u = new InputUnion
        //            {
        //                ki = new KeyboardInput
        //                {
        //                    wVk = 0,
        //                    wScan = 0x0D, // W
        //                    dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
        //                    dwExtraInfo = GetMessageExtraInfo(),
        //                },
        //            }
        //        },
        //    };
        //}


        //public static class SendKeys
        //{
        //    /// <summary>
        //    ///   Sends the specified key.
        //    /// </summary>
        //    /// <param name="key">The key.</param>
        //    public static void Send(Key key)
        //    {
        //        if (Keyboard.PrimaryDevice != null)
        //        {
        //            if (Keyboard.PrimaryDevice.ActiveSource != null)
        //            {
        //                var e1 = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Down) { RoutedEvent = Keyboard.KeyDownEvent };
        //                InputManager.Current.ProcessInput(e1);
        //            }
        //        }
        //    }
        //}

        //// Import the user32.dll
        //[DllImport("user32.dll", SetLastError = true)]
        //static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        //// Declare some keyboard keys as constants with its respective code
        //// See Virtual Code Keys: https://msdn.microsoft.com/en-us/library/dd375731(v=vs.85).aspx
        //public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        //public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        //public const int VK_RCONTROL = 0xA3; //Right Control key code

        //// Simulate a key press event
        //keybd_event(VK_RCONTROL, 0, KEYEVENTF_EXTENDEDKEY, 0);
        //keybd_event(VK_RCONTROL, 0, KEYEVENTF_KEYUP, 0);
    }
}
