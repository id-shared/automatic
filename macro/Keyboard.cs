using System;
using System.Runtime.InteropServices;

class Keyboard {
  public static bool Input(uint[] k, bool a) {
    INPUT[] inputs = new INPUT[k.Length];
    for (int i = 0; i < k.Length; i++) {
      inputs[i].type = I_TYPE;
      inputs[i].mkhi.ki.wVk = (ushort)k[i];
      inputs[i].mkhi.ki.wScan = 0;
      inputs[i].mkhi.ki.dwFlags = a ? E_KEYD : E_KEYU;
      inputs[i].mkhi.ki.time = 0;
      inputs[i].mkhi.ki.dwExtraInfo = IntPtr.Zero;
    }
    return SendInput((uint)inputs.Length, inputs, I_SIZE) != 0;
  }

  public static bool IsHeld(uint[] k) => k.All(key => (GetKeyState((int)key) & 0x8000) != 0);

  public static readonly uint E_KEYU = 0x0002;
  public static readonly uint E_KEYD = 0x0000;
  public static readonly uint I_TYPE = 1;
  public static readonly int I_SIZE = Marshal.SizeOf<INPUT>();

  [StructLayout(LayoutKind.Sequential)]
  private struct INPUT {
    public uint type;
    public MOUSEKEYBDHARDWAREINPUT mkhi;
  }

  [StructLayout(LayoutKind.Explicit)]
  private struct MOUSEKEYBDHARDWAREINPUT {
    [FieldOffset(0)] public MOUSEINPUT mi;
    [FieldOffset(0)] public KEYBDINPUT ki;
    [FieldOffset(0)] public HARDWAREINPUT hi;
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

  [DllImport("user32.dll")]
  private static extern short GetKeyState(int nVirtKey);
}
