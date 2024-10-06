using System.Runtime.InteropServices;

class Keyboard {
  public async static Task<bool> Hold(int t1, uint k1) {
    Input(k1, T);
    await Task.Delay(t1);
    Input(k1, F);
    return T;
  }

  public static bool Input(uint k1, bool @is) {
    I[0].type = INPUT_KEYBOARD;
    I[0].mkhi.ki.wVk = (ushort)k1;
    I[0].mkhi.ki.wScan = 0;
    I[0].mkhi.ki.dwFlags = @is ? 0 : KEYEVENTF_KEYUP;
    I[0].mkhi.ki.time = 0;
    I[0].mkhi.ki.dwExtraInfo = IntPtr.Zero;

    return SendInput((uint)I.Length, I, I_size) == 0 ? throw new ExternalException("SendInput failed.") : T;
  }

  private static readonly int I_size = Marshal.SizeOf(typeof(INPUT));
  private static readonly INPUT[] I = new INPUT[1];

  private const uint INPUT_KEYBOARD = 1;
  private const uint KEYEVENTF_KEYUP = 0x0002;
  private const bool F = false;
  private const bool T = true;

  [StructLayout(LayoutKind.Sequential)]
  private struct INPUT {
    public uint type;
    public MOUSEKEYBDHARDWAREINPUT mkhi;
  }

  [StructLayout(LayoutKind.Explicit)]
  private struct MOUSEKEYBDHARDWAREINPUT {
    [FieldOffset(0)]
    public MOUSEINPUT mi;
    [FieldOffset(0)]
    public KEYBDINPUT ki;
    [FieldOffset(0)]
    public HARDWAREINPUT hi;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct KEYBDINPUT {
    public ushort wVk;
    public ushort wScan;
    public uint dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct MOUSEINPUT {
    public int dx;
    public int dy;
    public uint mouseData;
    public uint dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct HARDWAREINPUT {
    public uint uMsg;
    public ushort wParamL;
    public ushort wParamH;
  }

  [DllImport("user32.dll")]
  private static extern uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);
}
