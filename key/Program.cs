using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Concurrent;

class Program {
  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
  private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

  private static readonly LowLevelKeyboardProc _keyboardProc = KeyboardHookCallback;
  private static readonly LowLevelMouseProc _mouseProc = MouseHookCallback;
  private static IntPtr _keyboardHookID = IntPtr.Zero;
  private static IntPtr _mouseHookID = IntPtr.Zero;
  private static readonly ConcurrentDictionary<string, bool> _keyStates = new();

  static async Task Main() {
    _keyboardHookID = await Task.Run(() => SetHook(_keyboardProc, WH_KEYBOARD_LL));
    _mouseHookID = await Task.Run(() => SetHook(_mouseProc, WH_MOUSE_LL));

    if (_keyboardHookID == IntPtr.Zero || _mouseHookID == IntPtr.Zero) {
      return;
    }

    Thread messageLoopThreads = new Thread(MonitorKeyStates);
    messageLoopThreads.IsBackground = true;
    messageLoopThreads.Start();

    MessageLoop();

    UnhookWindowsHookEx(_keyboardHookID);
    UnhookWindowsHookEx(_mouseHookID);
  }

  private static void MessageLoop() {
    MSG msg = new MSG();
    while (GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }
  }

  private static void MonitorKeyStates() {
    while (true) {
      Console.WriteLine("Current Key States:");
      foreach (var keyState in _keyStates) {
        Console.WriteLine($"{keyState.Key}: {keyState.Value}");
      }
      Thread.Sleep(1000);
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

    IntPtr hook = SetWindowsHookEx(hookType, proc, moduleHandle, 0);
    return hook;
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

  private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      switch ((int)wParam) {
        case WM_LBUTTONDOWN:
          _keyStates["LMB"] = true;
          break;
        case WM_LBUTTONUP:
          _keyStates["LMB"] = false;
          break;
      }
    }
    return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct POINT {
    public int x;
    public int y;
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

  private const int WH_KEYBOARD_LL = 13;
  private const int WH_MOUSE_LL = 14;
  private const int WM_KEYDOWN = 0x0100;
  private const int WM_SYSKEYDOWN = 0x0104;
  private const int WM_KEYUP = 0x0101;
  private const int WM_SYSKEYUP = 0x0105;
  private const int WM_LBUTTONDOWN = 0x0201;
  private const int WM_LBUTTONUP = 0x0202;

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
