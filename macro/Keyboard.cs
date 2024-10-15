using System.Runtime.InteropServices;

class Keyboard {
  public static bool Input(uint k1, bool @is) {
    INPUT[] inputs = new INPUT[1];
    inputs[0].type = INPUT_KEYBOARD;
    inputs[0].mkhi.ki.wVk = (ushort)k1;
    inputs[0].mkhi.ki.wScan = 0;
    inputs[0].mkhi.ki.dwFlags = @is ? 0 : KEYEVENTF_KEYUP;
    inputs[0].mkhi.ki.time = 0;
    inputs[0].mkhi.ki.dwExtraInfo = IntPtr.Zero;
    return SendInput((uint)inputs.Length, inputs, I_size) != 0;
  }

  static readonly int I_size = Marshal.SizeOf<INPUT>();
  const uint INPUT_KEYBOARD = 1;
  const uint KEYEVENTF_KEYUP = 0x0002;

  [StructLayout(LayoutKind.Sequential)]
  struct INPUT {
    public uint type;
    public MOUSEKEYBDHARDWAREINPUT mkhi;
  }

  [StructLayout(LayoutKind.Explicit)]
  struct MOUSEKEYBDHARDWAREINPUT {
    [FieldOffset(0)] public MOUSEINPUT mi;
    [FieldOffset(0)] public KEYBDINPUT ki;
    [FieldOffset(0)] public HARDWAREINPUT hi;
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
  extern static uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);
}
