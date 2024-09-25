﻿using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Concurrent;

class Program {
  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
  private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

  private static readonly LowLevelKeyboardProc d2_hook = D2HookCallback;
  private static readonly LowLevelMouseProc d1_hook = D1HookCallback;

  public static IntPtr d2_hook_id = IntPtr.Zero;
  public static IntPtr d1_hook_id = IntPtr.Zero;

  public static ConcurrentDictionary<uint, bool> Held = new() { };
  public static readonly bool F = false;
  public static readonly bool T = true;

  public static bool OnD2U(uint key) {
    Held[key] = F;
    return T switch {
      _ => T,
    };
  }

  public static bool OnD2D(uint key) {
    Held[key] = T;
    return T switch {
      _ => T,
    };
  }

  public static bool OnD1U(uint key) {
    Held[key] = F;
    return T switch {
      var _ when key == KeyM.L => D11U(),
      _ => T,
    };
  }

  public static bool OnD1D(uint key) {
    Task.Run(() => {
      Held[key] = T;
      return T switch {
        var _ when key == KeyM.L => D11D(key),
        _ => T,
      };
    });

    return T;
  }

  public static bool D11U() {
    Keyboard.Emulate(KeyE.C, F);
    Keyboard.Emulate(KeyA.D, F);
    Keyboard.Emulate(KeyA.U, F);
    Keyboard.Emulate(KeyA.R, F);
    Keyboard.Emulate(KeyA.L, F);
    return T;
  }

  public static bool D11D(uint key) {
    Keyboard.Emulate(KeyE.C, T);
    AtHeld(KeyA.D, Key.W, 100);
    AtHeld(KeyA.U, Key.S, 100);
    AtHeld(KeyA.R, Key.A, 100);
    AtHeld(KeyA.L, Key.D, 100);
    return T;
  }

  public static bool AtHeld(uint key_1, uint key, int time) {
    return T switch {
      var _ when IsHeld(key) => Keyboard.Hold(key_1, time),
      _ => T,
    };
  }

  public static bool IsHeld(uint key) {
    return Held.TryGetValue(key, out var isHeld) && isHeld;
  }

  public static IntPtr SetHook(Delegate proc, uint hookType) {
    using ProcessModule? module = Process.GetCurrentProcess().MainModule;

    switch (T) {
      case var _ when module == null:
        return IntPtr.Zero;
      default:
        IntPtr handle = GetModuleHandle(module.ModuleName);

        return T switch {
          var _ when handle == IntPtr.Zero => IntPtr.Zero,
          _ => SetWindowsHookEx((int)hookType, proc, handle, 0),
        };
    }
  }

  public static IntPtr D2HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      uint key = (uint)Marshal.ReadInt32(lParam);
      uint act = (uint)wParam;
      switch (T) {
        case var _ when act == WM_SYSKEYDOWN:
          OnD2D(key);
          return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
        case var _ when act == WM_KEYDOWN:
          OnD2D(key);
          return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
        case var _ when act == WM_SYSKEYUP:
          OnD2U(key);
          return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
        case var _ when act == WM_KEYUP:
          OnD2U(key);
          return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
        default:
          return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
      }
    }
    return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
  }

  public static IntPtr D1HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      uint act = (uint)wParam;
      switch (T) {
        case var _ when act == WM_LBUTTONDOWN:
          OnD1D(KeyM.L);
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
        case var _ when act == WM_LBUTTONUP:
          OnD1U(KeyM.L);
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
        case var _ when act == WM_RBUTTONDOWN:
          OnD1D(0x02);
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
        case var _ when act == WM_RBUTTONUP:
          OnD1U(0x02);
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
        default:
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
      }
    }
    return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
  }

  public static void Main() {
    d2_hook_id = SetHook(d2_hook, WH_KEYBOARD_LL);
    d1_hook_id = SetHook(d1_hook, WH_MOUSE_LL);

    Subscribe(new MSG());

    Detach(d2_hook_id);
    Detach(d1_hook_id);
  }

  private static void Subscribe(MSG msg) {
    while (GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }
  }

  private static bool Detach(nint id) {
    return T switch {
      var _ when id == IntPtr.Zero => F,
      _ => UnhookWindowsHookEx(id),
    };
  }

  private const uint WH_KEYBOARD_LL = 13;
  private const uint WH_MOUSE_LL = 14;
  private const uint WM_KEYDOWN = 0x0100;
  private const uint WM_SYSKEYDOWN = 0x0104;
  private const uint WM_KEYUP = 0x0101;
  private const uint WM_SYSKEYUP = 0x0105;
  private const uint WM_LBUTTONDOWN = 0x0201;
  private const uint WM_LBUTTONUP = 0x0202;
  private const uint WM_RBUTTONDOWN = 0x0204;
  private const uint WM_RBUTTONUP = 0x0205;
  private const uint KEYEVENTF_KEYUP = 0x0002;

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
