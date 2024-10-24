﻿using System.Diagnostics;
using System.Runtime.InteropServices;

class Perform {
  public static volatile Specter S3 = new(256);
  public static volatile Specter S2 = new(256);
  public static volatile Specter S1 = new(256);
  public static volatile Pattern P1 = new();
  public static volatile Device1 D1 = new("d1.dll");

  public static readonly uint[] RA = [KeyA.R];
  public static volatile bool R = A.F;

  public static readonly double LE = 209.9999;
  public static readonly double LC = 99.99999;
  public static readonly uint[] LA = [KeyA.L];
  public static volatile bool L = A.F;

  public static readonly int EY = 8;
  public static readonly int CY = 2;
  public static volatile int AY = 0;
  public static volatile int AX = 0;

  public static bool KeyEAU() {
    L = A.F;
    S1.TryEnqueue(_ => D1.L(A.F) && S2.TryEnqueue(_ => {
      AY = Upon(ci => !L && (0 <= ci) && D1.YXL(P1.YAxis(ci) * -CY, P1.XAxis(ci) / CY, A.F) && C(EY), AY) + 1;
      P1 = new Pattern();
      return A.T;
    }));
    return L;
  }

  public static bool KeyEAD() {
    L = L || S1.TryEnqueue(_ => D1.L(A.T) && S2.TryEnqueue(_ => {
      AY = Till(ci => L && (99 >= ci) && D1.YXL(P1.YAxis(ci) * CY, P1.XAxis(ci) / -CY, A.T) && C(EY), AY) - 1;
      return A.T;
    }));
    return L;
  }

  public static bool KeyDU() {
    return S1.TryEnqueue(_ => IO(LC, LA));
  }

  public static bool KeyDD() {
    return A.T;
  }

  public static bool KeyAU() {
    return S1.TryEnqueue(_ => IO(LC, RA));
  }

  public static bool KeyAD() {
    return A.T;
  }

  public static bool OnU(uint i) => i switch {
    KeyE.A => KeyEAU(),
    KeyX.D => KeyDU(),
    KeyX.A => KeyAU(),
    _ => A.F,
  };

  public static bool OnD(uint i) => i switch {
    KeyE.A => KeyEAD(),
    KeyX.D => KeyDD(),
    KeyX.A => KeyAD(),
    _ => A.T,
  };

  public static bool IX<X>(string e, X _) {
    Console.WriteLine($"{e}: {_}.");
    return A.T;
  }

  public static bool IO(double t, uint[] k) {
    I(k);
    Time.XO(t);
    O(k);
    return A.T;
  }

  public static bool O(uint[] k) => Device2.Input(k, A.F);

  public static bool I(uint[] k) => Device2.Input(k, A.T);

  public static bool C(double i) => Time.XO(i);

  public static IntPtr OnHookD2(int nCode, IntPtr wParam, IntPtr lParam) {
    IntPtr next = Native.CallNextHookEx(hookD2, nCode, wParam, lParam);
    if (nCode < 0) return next;

    uint key = (uint)Marshal.ReadInt32(lParam);

    switch ((uint)wParam) {
      case WM_SYSKEYDOWN or WM_KEYDOWN:
        if (key == KeyE.W) Exit();
        OnD(key);
        return next;
      case WM_SYSKEYUP or WM_KEYUP:
        OnU(key);
        return next;
      default:
        return next;
    }
  }

  public static IntPtr OnHookD1(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      switch ((uint)wParam) {
        case WM_MOUSEMOVE:
          S3.TryEnqueue(_ => {
            Native.POINT point = (Native.POINT)Marshal.PtrToStructure(lParam, typeof(Native.POINT));
            Console.WriteLine($"Mouse moved to X: {point.x}, Y: {point.y}");
            return A.T;
          });
          break;
        case WM_LBUTTONDOWN:
          S3.TryEnqueue(_ => {
            Console.WriteLine("Left mouse button down");
            return A.T;
          });
          break;
        case WM_LBUTTONUP:
          S3.TryEnqueue(_ => {
            Console.WriteLine("Left mouse button up");
            return A.T;
          });
          break;
      }
    }
    return Native.CallNextHookEx(hookD1, nCode, wParam, lParam);
  }

  public static int Upon(Func<int, bool> z, int i) {
    return z(i) ? Upon(z, i - 1) : i;
  }

  public static int Till(Func<int, bool> z, int i) {
    return z(i) ? Till(z, i + 1) : i;
  }

  public static void Exit() => Environment.Exit(0);

  public struct Back(int code, IntPtr w, IntPtr l, IntPtr i) {
    public IntPtr wParam = w, lParam = l, iParam = i;
    public int nCode = code;
  }

  public static IntPtr SetHook(Delegate proc, uint hookType) {
    using var module = Process.GetCurrentProcess().MainModule;
    if (module == null) return IntPtr.Zero;

    IntPtr handle = Native.GetModuleHandle(module.ModuleName);
    return handle == IntPtr.Zero ? IntPtr.Zero :
      Native.SetWindowsHookEx((int)hookType, proc, handle, 0);
  }

  public static void Subscribe(Native.MSG msg) {
    while (Native.GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      Native.TranslateMessage(ref msg);
      Native.DispatchMessage(ref msg);
    }
  }

  public Perform() {
    hookD2 = SetHook(onHookD2, 13);
    hookD1 = SetHook(onHookD1, 14);
    Subscribe(new Native.MSG());
  }

  public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
  public static readonly LowLevelProc onHookD2 = OnHookD2;
  public static readonly LowLevelProc onHookD1 = OnHookD1;
  public static volatile IntPtr hookD2 = IntPtr.Zero;
  public static volatile IntPtr hookD1 = IntPtr.Zero;

  public const uint WM_SYSKEYDOWN = 0x0104;
  public const uint WM_SYSKEYUP = 0x0105;
  public const uint WM_KEYDOWN = 0x0100;
  public const uint WM_KEYUP = 0x0101;

  public const uint WM_MOUSEMOVE = 0x0200;
  public const uint WM_LBUTTONDOWN = 0x0201;
  public const uint WM_LBUTTONUP = 0x0202;

}
