﻿using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Concurrent;

class Program {
  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
  private static readonly LowLevelKeyboardProc d1_hook = D1HookCallback;
  static ConcurrentDictionary<uint, bool> data = [];
  static IntPtr d1_hook_id = IntPtr.Zero;
  static readonly bool F = false;
  static readonly bool T = true;

  static async Task<bool> OnDown(uint key) {
    return F;
  }

  static async Task<bool> OnUp(uint key) {
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

  static IntPtr D1HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    switch (T) {
      case var _ when nCode >= 0:
        uint key = (uint)Marshal.ReadInt32(lParam);
        int act = (int)wParam;
        switch (T) {
          case var _ when act.Equals(WM_SYSKEYDOWN) || act.Equals(WM_KEYDOWN):
            Task.Run(() => OnDown(key));
            return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
          case var _ when act.Equals(WM_SYSKEYUP) || act.Equals(WM_KEYUP):
            Task.Run(() => OnUp(key));
            return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
          default:
            return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
        }
      default:
        return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
    }
  }

  static void Main() {
    d1_hook_id = SetHook(d1_hook, WH_KEYBOARD_LL);

    Subscribe(new MSG());

    Detach(d1_hook_id);
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

class Keyboard {
  public static uint I(uint key, bool is_pressed) {
    INPUT[] inputs = new INPUT[1];

    inputs[0].type = INPUT_KEYBOARD;
    inputs[0].mkhi.ki = new KEYBDINPUT {
      wVk = (ushort)key,
      wScan = 0,
      dwFlags = is_pressed ? 0 : KEYEVENTF_KEYUP,
      time = 0,
      dwExtraInfo = IntPtr.Zero
    };

    return SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
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

  [DllImport("user32.dll")]
  static extern uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);

  [DllImport("user32.dll")]
  static extern uint MapVirtualKey(uint uCode, uint uMapType);
}

//static bool Stop(ConcurrentDictionary<uint, bool> dict, uint key_3, uint key_2, uint key_1, uint key, bool is_pressed) {
//  switch (T) {
//    case var _ when dict.GetOrAdd(key_3, F) && key_2.Equals(key):
//      return Keyboard.SendKey(key_1, is_pressed);
//    case var _ when key_3.Equals(key) && dict.GetOrAdd(key_2, F):
//      return Keyboard.SendKey(key_1, is_pressed);
//    case var _ when key_3.Equals(key):
//      return Keyboard.SendKey(key_1, F);
//    default:
//      return F;
//  }
//}

//Console.WriteLine("Scanning for all ConsoleKey values based on scan codes...");
//uint startScanCode = 0x43;
//uint endScanCode = 0xFF;

//for (uint scanCode = startScanCode; scanCode <= endScanCode; scanCode++) {
//  Thread.Sleep(1000);
//  Console.WriteLine($"{scanCode}:");
//  Keyboard.SendKey((uint)ConsoleKey.C, T);
//  Keyboard.SendKey((uint)ConsoleKey.C, F);
//}
