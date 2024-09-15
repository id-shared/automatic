using System.Runtime.InteropServices;

class Keyboard {
  static readonly bool F = false;
  static readonly bool T = true;

  public static uint I(uint key, bool is_pressed) {
    INPUT[] inputs = new INPUT[1];

    inputs[0].type = INPUT_KEYBOARD;
    inputs[0].mkhi.ki = new KEYBDINPUT {
      wVk = (ushort)key,
      wScan = 0,
      dwFlags = is_pressed ? 0 : KEYEVENTF_KEYUP,
      time = 0,
      dwExtraInfo = IntPtr.Zero
    };

    return SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
  }

  const uint INPUT_KEYBOARD = 1;
  const uint KEYEVENTF_KEYUP = 0x0002;

  [StructLayout(LayoutKind.Sequential)]
  struct INPUT {
    public uint type;
    public MOUSEKEYBDHARDWAREINPUT mkhi;
  }

  [StructLayout(LayoutKind.Explicit)]
  struct MOUSEKEYBDHARDWAREINPUT {
    [FieldOffset(0)]
    public MOUSEINPUT mi;
    [FieldOffset(0)]
    public KEYBDINPUT ki;
    [FieldOffset(0)]
    public HARDWAREINPUT hi;
  }

  [StructLayout(LayoutKind.Sequential)]
  struct KEYBDINPUT {
    public ushort wVk;
    public ushort wScan;
    public uint dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
  }

  [StructLayout(LayoutKind.Sequential)]
  struct MOUSEINPUT {
    public int dx;
    public int dy;
    public uint mouseData;
    public uint dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
  }

  [StructLayout(LayoutKind.Sequential)]
  struct HARDWAREINPUT {
    public uint uMsg;
    public ushort wParamL;
    public ushort wParamH;
  }

  [DllImport("user32.dll")]
  static extern uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);

  [DllImport("user32.dll")]
  static extern uint MapVirtualKey(uint uCode, uint uMapType);
}

//public static bool Held(uint key) {
//  return (GetKeyState(key) & 0x8000) != 0;
//}


//[DllImport("user32.dll")]
//private static extern short GetKeyState(uint vKey);
