﻿using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq;

class Perform {
  private static bool D2UD() {
    WaitD = (int)Environment.TickCount64;
    return T;
  }

  private static bool D2UA() {
    WaitA = (int)Environment.TickCount64;
    return T;
  }

  private static bool D2DD() {
    return T;
  }

  private static bool D2DA() {
    return T;
  }

  private static bool D1UR() {
    return T;
  }

  private static bool D1UL() {
    return T;
  }

  private static bool D1DR() {
    return T;
  }

  private static bool D1DL() {
    return Do(() => {
      Reacted(WaitD, TimeD, [KeyA.L]);
      Reacted(WaitA, TimeA, [KeyA.R]);
      ReIO(103, [
        1,
        2,
        4,
        8,
        16,
      ], [KeyM.L], KeyE.A);
    });
  }

  private static volatile int WaitD = 0;
  private static volatile int WaitA = 0;
  private const int TimeD = 109;
  private const int TimeA = 109;

  private static bool Reacted(int t1, int t, uint[] n) {
    int time = (int)Environment.TickCount64 - t1;
    return t > time && n.All(_ => IO(t - time, _));
  }

  private static bool ReIO(int t1, int[] n1, uint[] n, uint k) {
    IO(1, k);
    Wait(t1 + n1.ElementAt(0));
    return n.All(IsHeld) ? ReIO(t1, Re(n1), n, k) : T;
  }

  private static int[] Re(int[] n) => n.Length > 1 ? n.Skip(1).ToArray() : [n.Last()];

  private static bool Do(Action z) {
    Task.Run(z);
    return T;
  }

  private static bool OnD2U(uint k) {
    Unit[k] = F;
    return T switch {
      var _ when KeyX.D == k => D2UD(),
      var _ when KeyX.A == k => D2UA(),
      _ => T,
    };
  }

  private static bool OnD2D(uint k) {
    Unit[k] = T;
    return T switch {
      var _ when KeyX.D == k => D2DD(),
      var _ when KeyX.A == k => D2DA(),
      var _ when KeyE.W == k => Exit(),
      _ => T,
    };
  }

  private static bool OnD1U(uint k) {
    Unit[k] = F;
    return T switch {
      var _ when KeyM.R == k => D1UR(),
      var _ when KeyM.L == k => D1UL(),
      _ => T,
    };
  }

  private static bool OnD1D(uint k) {
    Unit[k] = T;
    return T switch {
      var _ when KeyM.R == k => D1DR(),
      var _ when KeyM.L == k => D1DL(),
      _ => T,
    };
  }

  private static bool ReactIO(int t, uint[] n, uint k) {
    return !n.Any(IsHeld) && IO(t, k);
  }

  private static bool ReactO(uint[] n, uint k) {
    return !n.Any(IsHeld) && O(k);
  }

  private static bool ReactI(uint[] n, uint k) {
    return !n.Any(IsHeld) && I(k);
  }

  private static bool ActIO(int t, uint[] n, uint k) {
    return n.All(IsHeld) && IO(t, k);
  }

  private static bool ActO(uint[] n, uint k) {
    return n.All(IsHeld) && O(k);
  }

  private static bool ActI(uint[] n, uint k) {
    return n.All(IsHeld) && I(k);
  }

  private static bool IsHeld(uint k) {
    return Unit.TryGetValue(k, out bool is_held) && is_held;
  }

  private static readonly Dictionary<uint, bool> Unit = [];

  private static bool IO(int t, uint k) {
    I(k);
    Wait(t);
    O(k);
    return T;
  }

  private static bool O(uint k) {
    return Keyboard.Input(k, F);
  }

  private static bool I(uint k) {
    return Keyboard.Input(k, T);
  }

  private static void Wait(int i) {
    Thread.Sleep(i);
  }

  private static bool Exit() {
    Environment.Exit(0);
    return T;
  }

  public static IntPtr D2HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      uint key = (uint)Marshal.ReadInt32(lParam);
      uint act = (uint)wParam;
      _ = T switch {
        var _ when act == WM_SYSKEYDOWN => OnD2D(key),
        var _ when act == WM_KEYDOWN => OnD2D(key),
        var _ when act == WM_SYSKEYUP => OnD2U(key),
        var _ when act == WM_KEYUP => OnD2U(key),
        _ => T,
      };
    }
    return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
  }

  public static IntPtr D1HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      uint act = (uint)wParam;
      _ = T switch {
        var _ when act == WM_LBUTTONDOWN => OnD1D(KeyM.L),
        var _ when act == WM_LBUTTONUP => OnD1U(KeyM.L),
        var _ when act == WM_RBUTTONDOWN => OnD1D(KeyM.R),
        var _ when act == WM_RBUTTONUP => OnD1U(KeyM.R),
        _ => T,
      };
    }
    return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
  }

  private static IntPtr SetHook(Delegate proc, uint hookType) {
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

  private static void SubscribeKey(MSG msg) {
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
  public static void A() {
    d2_hook_id = SetHook(d2_hook, WH_KEYBOARD_LL);
    d1_hook_id = SetHook(d1_hook, WH_MOUSE_LL);

    SubscribeKey(new MSG());

    Detach(d2_hook_id);
    Detach(d1_hook_id);
  }

  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
  private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

  private static readonly LowLevelKeyboardProc d2_hook = D2HookCallback;
  private static readonly LowLevelMouseProc d1_hook = D1HookCallback;
  private static IntPtr d2_hook_id = IntPtr.Zero;
  private static IntPtr d1_hook_id = IntPtr.Zero;

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
  private const bool F = false;
  private const bool T = true;

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


  [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
  private static extern IntPtr GetModuleHandle(string lpModuleName);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool TranslateMessage(ref MSG lpMsg);

  [DllImport("user32.dll")]
  private static extern IntPtr DispatchMessage(ref MSG lpMsg);

  [DllImport("user32.dll")]
  private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
}
