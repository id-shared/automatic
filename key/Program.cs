using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Program {
  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
  private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

  private static readonly LowLevelKeyboardProc _keyboardProc = KeyboardHookCallback;
  private static readonly LowLevelMouseProc _mouseProc = MouseHookCallback;
  private static IntPtr _keyboardHookID = IntPtr.Zero;
  private static IntPtr _mouseHookID = IntPtr.Zero;
  private static readonly Dictionary<ConsoleKey, bool> _keyStates = new();
  private static bool _lmbPressed;

  static void Main() {
    _keyboardHookID = SetHook(_keyboardProc, WH_KEYBOARD_LL);
    _mouseHookID = SetHook(_mouseProc, WH_MOUSE_LL);

    if (_keyboardHookID == IntPtr.Zero || _mouseHookID == IntPtr.Zero) {
      Console.WriteLine("Failed to set hooks!");
      return;
    }

    Console.WriteLine("Hooks set successfully. Listening for keyboard and mouse events...");

    while (GetMessage(out MSG msg, IntPtr.Zero, 0, 0)) {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }

    UnhookWindowsHookEx(_keyboardHookID);
    UnhookWindowsHookEx(_mouseHookID);
  }

  private static IntPtr SetHook(Delegate proc, int hookType) {
    using ProcessModule curModule = Process.GetCurrentProcess().MainModule;
    if (curModule == null) {
      Console.WriteLine("Error: Could not get current module.");
      return IntPtr.Zero;
    }

    IntPtr moduleHandle = GetModuleHandle(curModule.ModuleName);
    if (moduleHandle == IntPtr.Zero) {
      Console.WriteLine("Error getting module handle: " + Marshal.GetLastWin32Error());
      return IntPtr.Zero;
    }

    IntPtr hook = SetWindowsHookEx(hookType, proc, moduleHandle, 0);
    if (hook == IntPtr.Zero) {
      Console.WriteLine($"Error setting hook: {Marshal.GetLastWin32Error()}");
    }

    return hook;
  }

  private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      var key = (ConsoleKey)Marshal.ReadInt32(lParam);

      Console.WriteLine($"Pressed key: {key}");
      switch ((int)wParam) {
        case WM_KEYDOWN:
        case WM_SYSKEYDOWN:
          _keyStates[key] = true;
          if (_lmbPressed && _keyStates.ContainsKey(ConsoleKey.A) && _keyStates[ConsoleKey.A]) {
            SimulateKeyPress(ConsoleKey.L);
          }
          break;

        case WM_KEYUP:
        case WM_SYSKEYUP:
          _keyStates[key] = false;
          if (_lmbPressed && _keyStates.ContainsKey(ConsoleKey.A) && _keyStates[ConsoleKey.A]) {
            SimulateKeyPress(ConsoleKey.L);
          } else if (key == ConsoleKey.A || key == ConsoleKey.L) {
            SimulateKeyRelease(ConsoleKey.L);
          }
          break;
      }
    }
    return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);
  }

  private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      switch ((int)wParam) {
        case WM_LBUTTONDOWN:
          _lmbPressed = true;
          Console.WriteLine("LMB Pressed");
          break;

        case WM_LBUTTONUP:
          _lmbPressed = false;
          Console.WriteLine("LMB Released");
          break;
      }
    }
    return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
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

  private static void SimulateKeyPress(ConsoleKey key) => SimulateKey(key, true);
  private static void SimulateKeyRelease(ConsoleKey key) => SimulateKey(key, false);

  private const int WH_KEYBOARD_LL = 13;
  private const int WH_MOUSE_LL = 14;
  private const int WM_KEYDOWN = 0x0100;
  private const int WM_SYSKEYDOWN = 0x0104;
  private const int WM_KEYUP = 0x0101;
  private const int WM_SYSKEYUP = 0x0105;
  private const int WM_LBUTTONDOWN = 0x0201;
  private const int WM_LBUTTONUP = 0x0202;
  private const int INPUT_KEYBOARD = 1;
  private const int KEYEVENTF_KEYUP = 0x0002;

  [StructLayout(LayoutKind.Sequential)]
  private struct INPUT {
    public int type;
    public InputUnion u;
  }

  [StructLayout(LayoutKind.Explicit)]
  private struct InputUnion {
    [FieldOffset(0)]
    public MOUSEINPUT mi;
    [FieldOffset(0)]
    public KEYBDINPUT ki;
    [FieldOffset(0)]
    public HARDWAREINPUT hi;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct MOUSEINPUT {
    public int dx;
    public int dy;
    public int mouseData;
    public int dwFlags;
    public int time;
    public IntPtr dwExtraInfo;
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
  private struct HARDWAREINPUT {
    public int uMsg;
    public ushort wParamL;
    public ushort wParamH;
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
