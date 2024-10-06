﻿using System.Runtime.InteropServices;

public class Handler {
  public static IntPtr D2HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      Perform.EnqueueTask(() => {
        uint key = (uint)Marshal.ReadInt32(lParam);
        uint act = (uint)wParam;
        _ = T switch {
          var _ when act == WM_SYSKEYDOWN => OnD2D(key),
          var _ when act == WM_KEYDOWN => OnD2D(key),
          var _ when act == WM_SYSKEYUP => OnD2U(key),
          var _ when act == WM_KEYUP => OnD2U(key),
          _ => T,
        };
      });
    }
    return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
  }

  public static IntPtr D1HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      Perform.EnqueueTask(() => {
        uint act = (uint)wParam;
        _ = T switch {
          var _ when act == WM_LBUTTONDOWN => OnD1D(KeyM.L),
          var _ when act == WM_LBUTTONUP => OnD1U(KeyM.L),
          var _ when act == WM_RBUTTONDOWN => OnD1D(0x02),
          var _ when act == WM_RBUTTONUP => OnD1U(0x02),
          _ => T,
        };
      });
    }
    return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
  }

  public static IntPtr d2_hook_id = IntPtr.Zero;
  public static IntPtr d1_hook_id = IntPtr.Zero;

  private static bool D2UD() {
    TimeD = (int)Environment.TickCount64;
    return O(KeyA.L);
  }

  private static bool D2UA() {
    TimeA = (int)Environment.TickCount64;
    return O(KeyA.R);
  }

  private static bool D2DD() {
    return T;
  }

  private static bool D2DA() {
    return T;
  }

  private static bool D1UL() {
    ReactO(KeyM.L, KeyA.R);
    ReactO(KeyM.L, KeyA.L);
    ReactO(KeyM.L, KeyE.C);
    ReactO(KeyM.L, KeyE.A);
    return T;
  }

  private static bool D1DL() {
    Reactor(109, TimeD, KeyA.L);
    Reactor(109, TimeA, KeyA.R);
    IO(9, KeyE.A);
    ActI(KeyM.L, KeyE.A);
    C(49);
    ActI(KeyM.L, KeyE.C);
    return T;
  }

  private static bool Reactor(int t1, int t, uint k) {
    int time = (int)Environment.TickCount64 - t;
    return t1 > time ? IO(t1, k) : T;
  }

  private static bool ReactIO(int t, uint k1, uint k) {
    return IsHeld(k1) ? T : IO(t, k);
  }

  private static bool ReactO(uint k1, uint k) {
    return IsHeld(k1) ? T : O(k);
  }

  private static bool ReactI(uint k1, uint k) {
    return IsHeld(k1) ? T : I(k);
  }

  private static bool ActIO(int t, uint k1, uint k) {
    return IsHeld(k1) ? IO(t, k) : T;
  }

  private static bool ActO(uint k1, uint k) {
    return IsHeld(k1) ? O(k) : T;
  }

  private static bool ActI(uint k1, uint k) {
    return IsHeld(k1) ? I(k) : T;
  }

  private static bool IO(int t, uint k) {
    I(k);
    C(t);
    O(k);
    return T;
  }

  private static bool O(uint k) {
    return IsHeld(k) ? Keyboard.Input(k, F) : T;
  }

  private static bool I(uint k) {
    return IsHeld(k) ? T : Keyboard.Input(k, T);
  }

  private static void C(int i) {
    Thread.Sleep(i);
  }

  private static bool IsHeld(uint k) {
    return Unit.TryGetValue(k, out bool is_held) && is_held;
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
      _ => T,
    };
  }

  private static bool OnD1U(uint k) {
    Unit[k] = F;
    return T switch {
      var _ when KeyM.L == k => D1UL(),
      _ => T,
    };
  }

  private static bool OnD1D(uint k) {
    Unit[k] = T;
    return T switch {
      var _ when KeyM.L == k => D1DL(),
      _ => T,
    };
  }

  private static readonly Dictionary<uint, bool> Unit = [];
  private static int TimeD = 0;
  private static int TimeA = 0;

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


  [DllImport("user32.dll")]
  private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
}
