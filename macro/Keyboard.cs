using System.Runtime.InteropServices;

class Keyboard {
  public static bool Input(uint k1, bool @is) {
    INPUT[] I = new INPUT[1];
    I[0].type = 1;
    I[0].mkhi.ki.wVk = (ushort)k1;
    I[0].mkhi.ki.wScan = 0;
    I[0].mkhi.ki.dwFlags = @is ? 0 : (uint)2;
    I[0].mkhi.ki.time = 0;
    I[0].mkhi.ki.dwExtraInfo = IntPtr.Zero;
    return SendInput((uint)I.Length, I, Marshal.SizeOf<INPUT>()) != 0;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct INPUT {
    public uint type;
    public MOUSEKEYBDHARDWAREINPUT mkhi;
  }

  [StructLayout(LayoutKind.Explicit)]
  public struct MOUSEKEYBDHARDWAREINPUT {
    [FieldOffset(0)]
    public MOUSEINPUT mi;
    [FieldOffset(0)]
    public KEYBDINPUT ki;
    [FieldOffset(0)]
    public HARDWAREINPUT hi;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct KEYBDINPUT {
    public ushort wVk;
    public ushort wScan;
    public uint dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct MOUSEINPUT {
    public int dx;
    public int dy;
    public uint mouseData;
    public uint dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct HARDWAREINPUT {
    public uint uMsg;
    public ushort wParamL;
    public ushort wParamH;
  }

  [DllImport("user32.dll")]
  public static extern uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);
}
