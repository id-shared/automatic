using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Program {
  // Define the callback delegate
  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

  private static LowLevelKeyboardProc _proc = HookCallback;
  private static IntPtr _hookID = IntPtr.Zero;

  // Dictionary to keep track of key states
  private static Dictionary<ConsoleKey, bool> _keyStates = new Dictionary<ConsoleKey, bool>();
  private static ConsoleKey _keyToHold = ConsoleKey.None;

  static void Main(string[] args) {
    _hookID = SetHook(_proc);

    if (_hookID == IntPtr.Zero) {
      Console.WriteLine("Failed to set hook!");
    } else {
      Console.WriteLine("Hook set successfully. Listening for key events...");
    }

    // Run message loop to keep the application alive and listen to hook events
    MSG msg;
    while (GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }

    UnhookWindowsHookEx(_hookID);  // This won't be hit but is included for completeness
  }

  private static IntPtr SetHook(LowLevelKeyboardProc proc) {
    using (Process curProcess = Process.GetCurrentProcess())
    using (ProcessModule curModule = curProcess.MainModule) {
      if (curModule == null) {
        Console.WriteLine("Error: Could not get current module.");
        return IntPtr.Zero;
      }

      IntPtr moduleHandle = GetModuleHandle(curModule.ModuleName);
      if (moduleHandle == IntPtr.Zero) {
        Console.WriteLine("Error getting module handle: " + Marshal.GetLastWin32Error());
        return IntPtr.Zero;
      }

      IntPtr hook = SetWindowsHookEx(WH_KEYBOARD_LL, proc, moduleHandle, 0);
      if (hook == IntPtr.Zero) {
        Console.WriteLine("Error setting hook: " + Marshal.GetLastWin32Error());
      }
      return hook;
    }
  }

  private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      int vkCode = Marshal.ReadInt32(lParam);
      ConsoleKey key = (ConsoleKey)vkCode;

      switch ((int)wParam) {
        case WM_KEYDOWN:
        case WM_SYSKEYDOWN:
          if (!_keyStates.ContainsKey(key)) {
            _keyStates[key] = true;
          }
          Console.WriteLine("Key down: " + key);

          // Check if the key pressed is the one to hold
          if (key == ConsoleKey.H) {
            _keyToHold = ConsoleKey.J;  // Example: Hold key J when H is pressed
            SimulateKeyPress(_keyToHold);
          }
          break;

        case WM_KEYUP:
        case WM_SYSKEYUP:
          if (_keyStates.ContainsKey(key)) {
            _keyStates[key] = false;
          }
          Console.WriteLine("Key up: " + key);

          // Release the key if it was the one being held
          if (key == _keyToHold) {
            _keyToHold = ConsoleKey.None;
            SimulateKeyRelease(ConsoleKey.J);  // Release the held key
          }
          break;
      }
    }
    return CallNextHookEx(_hookID, nCode, wParam, lParam);
  }

  private static void SimulateKeyPress(ConsoleKey key) {
    INPUT input = new INPUT {
      type = INPUT_KEYBOARD,
      u = new InputUnion {
        ki = new KEYBDINPUT {
          wVk = (ushort)key,
          dwFlags = 0, // 0 for key press
          dwExtraInfo = IntPtr.Zero
        }
      }
    };
    SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
    Console.WriteLine("Simulated key press: " + key);
  }

  private static void SimulateKeyRelease(ConsoleKey key) {
    INPUT input = new INPUT {
      type = INPUT_KEYBOARD,
      u = new InputUnion {
        ki = new KEYBDINPUT {
          wVk = (ushort)key,
          dwFlags = KEYEVENTF_KEYUP, // Key release
          dwExtraInfo = IntPtr.Zero
        }
      }
    };
    SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
    Console.WriteLine("Simulated key release: " + key);
  }

  // P/Invoke declarations
  private const int WH_KEYBOARD_LL = 13;
  private const int WM_KEYDOWN = 0x0100;
  private const int WM_SYSKEYDOWN = 0x0104;
  private const int WM_KEYUP = 0x0101;
  private const int WM_SYSKEYUP = 0x0105;

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
  private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

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
