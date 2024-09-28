using System.Runtime.InteropServices;

class Keyboard {
  public async static Task<bool> Hold(uint key, int time) {
    Input(key, T);
    await Task.Delay(time);
    Input(key, F);
    return T;
  }

  public static bool Input(uint key, bool is_pressed) {
    I[0].type = INPUT_KEYBOARD;
    I[0].mkhi.ki.wVk = (ushort)key;
    I[0].mkhi.ki.wScan = 0;
    I[0].mkhi.ki.dwFlags = is_pressed ? 0 : KEYEVENTF_KEYUP;
    I[0].mkhi.ki.time = 0;
    I[0].mkhi.ki.dwExtraInfo = IntPtr.Zero;

    SendInput((uint)I.Length, I, I_size);

    return T;
  }

  private static readonly int I_size = Marshal.SizeOf(typeof(INPUT));
  private static readonly INPUT[] I = new INPUT[1];
  private static readonly bool F = false;
  private static readonly bool T = true;

  private const uint INPUT_KEYBOARD = 1;
  private const uint KEYEVENTF_KEYUP = 0x0002;

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
