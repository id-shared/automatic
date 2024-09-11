using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Concurrent;

class Program {
  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
  private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

  private static readonly LowLevelKeyboardProc d1_hook = D1HookCallback;
  private static readonly LowLevelMouseProc d2_hook = D2HookCallback;

  static ConcurrentDictionary<uint, bool> data = new ConcurrentDictionary<uint, bool>();
  static IntPtr d1_hook_id = IntPtr.Zero;
  static IntPtr d2_hook_id = IntPtr.Zero;
  static readonly bool F = false;
  static readonly bool T = true;

  static async Task<bool> OnD2Down() {
    // Handle mouse down event here
    Console.WriteLine("Mouse button pressed.");
    return T;
  }

  static async Task<bool> OnD2Up() {
    // Handle mouse up event here
    Console.WriteLine("Mouse button released.");
    return T;
  }

  static async Task<bool> OnD1Down(uint key) {
    return F;
  }

  static async Task<bool> OnD1Up(uint key) {
    switch (T) {
      case var _ when key.Equals((uint)ConsoleKey.A):
        return await Move((uint)ConsoleKey.RightArrow);
      case var _ when key.Equals((uint)ConsoleKey.D):
        return await Move((uint)ConsoleKey.LeftArrow);
      case var _ when key.Equals((uint)ConsoleKey.W):
        return await Move((uint)ConsoleKey.DownArrow);
      case var _ when key.Equals((uint)ConsoleKey.S):
        return await Move((uint)ConsoleKey.UpArrow);
      default:
        return F;
    };
  }

  static async Task<bool> Move(uint key) {
    return await Task.Run(() => {
      Keyboard.I(key, T);
      Thread.Sleep(100);
      Keyboard.I(key, F);

      return T;
    });
  }

  static void Subscribe(MSG msg) {
    while (GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }
  }

  static void Detach(nint id) {
    if (id != IntPtr.Zero) {
      UnhookWindowsHookEx(id);
    }
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

  static IntPtr D1HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      uint key = (uint)Marshal.ReadInt32(lParam);
      int act = (int)wParam;
      switch (act) {
        case WM_SYSKEYDOWN:
        case WM_KEYDOWN:
          Task.Run(() => OnD1Down(key));
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
        case WM_SYSKEYUP:
        case WM_KEYUP:
          Task.Run(() => OnD1Up(key));
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
        default:
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
      }
    }
    return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
  }

  static IntPtr D2HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      int act = (int)wParam;
      switch (act) {
        case WM_LBUTTONDOWN:
        case WM_LBUTTONUP:
          Console.WriteLine("Left mouse button up.");
          Task.Run(() => OnD2Up());
          return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
        case WM_RBUTTONDOWN:
        case WM_RBUTTONUP:
          Console.WriteLine("Right mouse button up.");
          Task.Run(() => OnD2Up());
          return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
        default:
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
      }
    }
    return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
  }

  static void Main() {
    //d1_hook_id = SetHook(d1_hook, WH_KEYBOARD_LL);
    d2_hook_id = SetHook(d2_hook, WH_MOUSE_LL);

    Subscribe(new MSG());

    Detach(d1_hook_id);
    Detach(d2_hook_id);
  }

  private const int WH_KEYBOARD_LL = 13;
  private const int WH_MOUSE_LL = 14;
  private const int WM_KEYDOWN = 0x0100;
  private const int WM_SYSKEYDOWN = 0x0104;
  private const int WM_KEYUP = 0x0101;
  private const int WM_SYSKEYUP = 0x0105;
  private const int WM_LBUTTONDOWN = 0x0201;
  private const int WM_LBUTTONUP = 0x0202;
  private const int WM_RBUTTONDOWN = 0x0204;
  private const int WM_RBUTTONUP = 0x0205;
  private const int KEYEVENTF_KEYUP = 0x0002;

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

  [DllImport("user32.dll")]
  private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool UnhookWindowsHookEx(IntPtr hhk);

  [DllImport("user32.dll")]
  private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

  [DllImport("kernel32.dll")]
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
