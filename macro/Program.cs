using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Concurrent;

class Program {
  static readonly ConcurrentDictionary<ConsoleKey, bool> state = new();
  static IntPtr hook_id = IntPtr.Zero;

  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
  private static readonly LowLevelKeyboardProc hook = KeyboardHookCallback;

  static bool Every(ConcurrentDictionary<ConsoleKey, bool> dict, ConsoleKey key, bool is_press) {
    switch (true) {
      case var _ when key.Equals(ConsoleKey.W):
        switch (true) {
          case var _ when dict.GetOrAdd(ConsoleKey.V, false):
            return Keyboard.SendKey(ConsoleKey.K, is_press);
          default:
            return false;
        }
      case var _ when key.Equals(ConsoleKey.S):
        switch (true) {
          case var _ when dict.GetOrAdd(ConsoleKey.V, false):
            return Keyboard.SendKey(ConsoleKey.I, is_press);
          default:
            return false;
        }
      case var _ when key.Equals(ConsoleKey.D):
        switch (true) {
          case var _ when dict.GetOrAdd(ConsoleKey.V, false):
            return Keyboard.SendKey(ConsoleKey.J, is_press);
          default:
            return false;
        }
      case var _ when key.Equals(ConsoleKey.A):
        switch (true) {
          case var _ when dict.GetOrAdd(ConsoleKey.V, false):
            return Keyboard.SendKey(ConsoleKey.L, is_press);
          default:
            return false;
        }
      default:
        return false;
    }
  }

  static void Subscribe(MSG msg) {
    while (GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }
  }

  static void Detach(nint id) {
    UnhookWindowsHookEx(id);
  }

  static IntPtr SetHook(Delegate proc, int hookType) {
    using ProcessModule? curModule = Process.GetCurrentProcess().MainModule;
    if (curModule == null) {
      return IntPtr.Zero;
    }

    IntPtr moduleHandle = GetModuleHandle(curModule.ModuleName);
    if (moduleHandle == IntPtr.Zero) {
      return IntPtr.Zero;
    }

    return SetWindowsHookEx(hookType, proc, moduleHandle, 0);
  }

  static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      ConsoleKey key = (ConsoleKey)Marshal.ReadInt32(lParam);
      switch ((int)wParam) {
        case WM_SYSKEYUP:
        case WM_KEYUP:
          state[key] = false;
          Every(state, key, false);
          break;
        case WM_SYSKEYDOWN:
        case WM_KEYDOWN:
          state[key] = true;
          Every(state, key, true);
          break;
      }
    }
    return CallNextHookEx(hook_id, nCode, wParam, lParam);
  }

  static void Main() {
    hook_id = SetHook(hook, WH_KEYBOARD_LL);

    Subscribe(new MSG());

    Detach(hook_id);
  }

  private const int WH_KEYBOARD_LL = 13;
  private const int WM_KEYDOWN = 0x0100;
  private const int WM_SYSKEYDOWN = 0x0104;
  private const int WM_KEYUP = 0x0101;
  private const int WM_SYSKEYUP = 0x0105;
  private const int KEYEVENTF_KEYUP = 0x0002;
  private const int INPUT_KEYBOARD = 1;

  [StructLayout(LayoutKind.Sequential)]
  private struct MSG {
    public IntPtr hWnd;
    public uint message;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint time;
    public POINT pt;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct POINT {
    public int x;
    public int y;
  }

  [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

  [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool UnhookWindowsHookEx(IntPtr hhk);

  [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

  [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
  private static extern IntPtr GetModuleHandle(string lpModuleName);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool TranslateMessage(ref MSG lpMsg);

  [DllImport("user32.dll")]
  private static extern IntPtr DispatchMessage(ref MSG lpMsg);
}

class Keyboard {
  [DllImport("user32.dll", SetLastError = true)]
  static extern uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);

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

  const uint INPUT_KEYBOARD = 1;
  const uint KEYEVENTF_KEYUP = 0x0002;

  public static bool SendKey(ConsoleKey key, bool is_press) {
    INPUT[] inputs = new INPUT[2];

    inputs[0].type = INPUT_KEYBOARD;
    inputs[0].mkhi.ki = new KEYBDINPUT {
      wVk = Convert.ConsoleKeyToVkCode (key),
      wScan = 0,
      dwFlags = is_press ? 0 : KEYEVENTF_KEYUP,
      time = 0,
      dwExtraInfo = IntPtr.Zero
    };

    SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

    return is_press;
  }
}

class Convert {
  public static ushort ConsoleKeyToVkCode(ConsoleKey key) {
    uint scanCode = ConsoleKeyToScanCode(key);
    return ScanCodeToVkCode(scanCode);
  }

  public static ushort ScanCodeToVkCode(uint scanCode) {
    return (ushort)MapVirtualKey(scanCode, MAPVK_VSC_TO_VK);
  }

  public static uint ConsoleKeyToScanCode(ConsoleKey key) {
    // Map ConsoleKey to scan code manually or via a dictionary
    switch (key) {
      case ConsoleKey.A: return 0x1E;
      case ConsoleKey.B: return 0x30;
      case ConsoleKey.C: return 0x2E;
      case ConsoleKey.D: return 0x20;
      case ConsoleKey.E: return 0x12;
      case ConsoleKey.F: return 0x21;
      case ConsoleKey.G: return 0x22;
      case ConsoleKey.H: return 0x23;
      case ConsoleKey.I: return 0x17;
      case ConsoleKey.J: return 0x24;
      case ConsoleKey.K: return 0x25;
      case ConsoleKey.L: return 0x26;
      case ConsoleKey.M: return 0x32;
      case ConsoleKey.N: return 0x31;
      case ConsoleKey.O: return 0x18;
      case ConsoleKey.P: return 0x19;
      case ConsoleKey.Q: return 0x10;
      case ConsoleKey.R: return 0x13;
      case ConsoleKey.S: return 0x1F;
      case ConsoleKey.T: return 0x14;
      case ConsoleKey.U: return 0x16;
      case ConsoleKey.V: return 0x2F;
      case ConsoleKey.W: return 0x11;
      case ConsoleKey.X: return 0x2D;
      case ConsoleKey.Y: return 0x15;
      case ConsoleKey.Z: return 0x2C;
      case ConsoleKey.D0: return 0x0B;
      case ConsoleKey.D1: return 0x02;
      case ConsoleKey.D2: return 0x03;
      case ConsoleKey.D3: return 0x04;
      case ConsoleKey.D4: return 0x05;
      case ConsoleKey.D5: return 0x06;
      case ConsoleKey.D6: return 0x07;
      case ConsoleKey.D7: return 0x08;
      case ConsoleKey.D8: return 0x09;
      case ConsoleKey.D9: return 0x0A;
      case ConsoleKey.Enter: return 0x1C;
      case ConsoleKey.Escape: return 0x01;
      // Add other keys as needed
      default: throw new ArgumentException($"No scan code found for ConsoleKey.{key}");
    }
  }

  const uint MAPVK_VSC_TO_VK = 0;

  [DllImport("user32.dll", SetLastError = true)]
  static extern uint MapVirtualKey(uint uCode, uint uMapType);
}
