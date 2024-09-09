﻿using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Windows.Forms;

class Program {
  static readonly ConcurrentDictionary<ConsoleKey, bool> state = new();
  static IntPtr hook_id = IntPtr.Zero;

  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
  private static readonly LowLevelKeyboardProc hook = KeyboardHookCallback;

  static bool Every(ConcurrentDictionary<ConsoleKey, bool> dict, ConsoleKey key, bool is_press) {
    switch (true) {
      case var _ when key.Equals(ConsoleKey.V) || key.Equals(ConsoleKey.W) || key.Equals(ConsoleKey.S) || key.Equals(ConsoleKey.D) || key.Equals(ConsoleKey.A):
        Each(dict, ConsoleKey.V, ConsoleKey.W, ConsoleKey.K);
        Each(dict, ConsoleKey.V, ConsoleKey.S, ConsoleKey.I);
        Each(dict, ConsoleKey.V, ConsoleKey.D, ConsoleKey.J);
        Each(dict, ConsoleKey.V, ConsoleKey.A, ConsoleKey.L);
        return true;
      default:
        return false;
    }

    return true;
  }

  static bool Each(ConcurrentDictionary<ConsoleKey, bool> dict, ConsoleKey key_2, ConsoleKey key_1, ConsoleKey key) {
    switch (true) {
      case var _ when dict.GetOrAdd(key_2, false) && dict.GetOrAdd(key_1, false):
        return Keyboard.SendKey((uint)key, true);
      case var _ when dict.GetOrAdd(key_1, false):
        return Keyboard.SendKey((uint)key, false);
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
      int scanCode = Marshal.ReadInt32(lParam);
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
  public static bool SendKey(uint key, bool is_press) {
    INPUT[] inputs = new INPUT[1];

    inputs[0].type = INPUT_KEYBOARD;
    inputs[0].mkhi.ki = new KEYBDINPUT {
      wVk = (ushort)key,
      wScan = 0,
      dwFlags = is_press ? 0 : KEYEVENTF_KEYUP,
      time = 0,
      dwExtraInfo = IntPtr.Zero
    };

    SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

    return is_press;
  }

  public static ushort ConsoleKeyToVkCode(ConsoleKey keys) {
    Console.WriteLine($"scancode: {(uint)keys:X}");
    foreach (ConsoleKey key in Enum.GetValues(typeof(ConsoleKey))) {
      uint virtualKeyCode = (uint)key;

      // Convert the virtual key code to a scan code
      uint scanCode = MapVirtualKey(virtualKeyCode, 0x01);

      // Output the ConsoleKey and its corresponding scan code
      Console.WriteLine($"ConsoleKey: {key}, Scan Code: {scanCode:X}");
    }
    return (ushort)MapVirtualKey(0x2E, 0x01);
  }

  const uint INPUT_KEYBOARD = 1;
  const uint KEYEVENTF_KEYUP = 0x0002;

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

  [DllImport("user32.dll", SetLastError = true)]
  static extern uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);

  [DllImport("user32.dll", SetLastError = true)]
  static extern uint MapVirtualKey(uint uCode, uint uMapType);
}

//Console.WriteLine("Scanning for all ConsoleKey values based on scan codes...");
//uint startScanCode = 0x43;
//uint endScanCode = 0xFF;

//for (uint scanCode = startScanCode; scanCode <= endScanCode; scanCode++) {
//  Thread.Sleep(1000);
//  Console.WriteLine($"{scanCode}:");
//  Keyboard.SendKey((uint)ConsoleKey.C, true);
//  Keyboard.SendKey((uint)ConsoleKey.C, false);
//}