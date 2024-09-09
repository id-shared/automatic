using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

class Program {
  private static readonly ConcurrentDictionary<string, bool> _keyStates = new();
  private static IntPtr _keyboardHookID = IntPtr.Zero;

  private static readonly LowLevelKeyboardProc _keyboardProc = KeyboardHookCallback;
  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

  static async Task Main() {
    _keyboardHookID = SetHook(_keyboardProc, WH_KEYBOARD_LL);

    var monitorStatesTask = Task.Run(() => MonitorStatesAsync());

    await MessageLoopAsync();

    UnhookWindowsHookEx(_keyboardHookID);
  }

  private static async Task MonitorStatesAsync() {
    while (true) {
      Console.Clear();
      Console.WriteLine("Current Key States:");
      foreach (var keyState in _keyStates) {
        Console.WriteLine($"{keyState.Key}: {keyState.Value}");
      }
      await Task.Delay(10); // Use Task.Delay instead of Thread.Sleep
    }
  }

  private static async Task MessageLoopAsync() {
    MSG msg = new MSG();
    while (GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }
  }

  private static IntPtr SetHook(Delegate proc, int hookType) {
    using ProcessModule curModule = Process.GetCurrentProcess().MainModule;
    if (curModule == null) {
      return IntPtr.Zero;
    }

    IntPtr moduleHandle = GetModuleHandle(curModule.ModuleName);
    if (moduleHandle == IntPtr.Zero) {
      return IntPtr.Zero;
    }

    return SetWindowsHookEx(hookType, proc, moduleHandle, 0);
  }

  private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      string key = ((ConsoleKey)Marshal.ReadInt32(lParam)).ToString();
      switch ((int)wParam) {
        case WM_KEYDOWN:
        case WM_SYSKEYDOWN:
          _keyStates[key] = true;
          break;
        case WM_KEYUP:
        case WM_SYSKEYUP:
          _keyStates[key] = false;
          break;
      }
    }
    return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
  }

  private static void SimulateKey(ConsoleKey key, bool isPress) {
    INPUT input = new INPUT {
      type = INPUT_KEYBOARD,
      u = new InputUnion {
        ki = new KEYBDINPUT {
          wVk = (ushort)key,
          dwFlags = isPress ? 0 : KEYEVENTF_KEYUP,
          dwExtraInfo = IntPtr.Zero
        }
      }
    };
    SendInput(1, new[] { input }, Marshal.SizeOf<INPUT>());
    Console.WriteLine($"Simulated key {(isPress ? "press" : "release")}: {key}");
  }

  private static void SimulateKeyRelease(ConsoleKey key) => SimulateKey(key, false);
  private static void SimulateKeyPress(ConsoleKey key) => SimulateKey(key, true);

  private const int WH_KEYBOARD_LL = 13;
  private const int WM_KEYDOWN = 0x0100;
  private const int WM_SYSKEYDOWN = 0x0104;
  private const int WM_KEYUP = 0x0101;
  private const int WM_SYSKEYUP = 0x0105;
  private const int KEYEVENTF_KEYUP = 0x0002;
  private const int INPUT_KEYBOARD = 1;

  [StructLayout(LayoutKind.Sequential)]
  private struct INPUT {
    public int type;
    public InputUnion u;
  }

  [StructLayout(LayoutKind.Explicit)]
  private struct InputUnion {
    [FieldOffset(0)]
    public KEYBDINPUT ki;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct KEYBDINPUT {
    public ushort wVk;
    public ushort wScan;
    public int dwFlags;
    public int time;
    public IntPtr dwExtraInfo;
  }

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
  private static extern uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);

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
