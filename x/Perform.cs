using System.Diagnostics;
using System.Runtime.InteropServices;

class Perform {
  public static bool KeyForwardSlashU() {
    L = A.F;
    P1.TryEnqueue(_ => X2.E1(A.F) && P2.TryEnqueue(_ => {
      AY = Upon(ci => !L && (0 <= ci) && X2.YX(PY.YAxis(ci) * -CY, PX.XAxis(ci) / CY) && Time.XO(EY), AY) + 1;
      PY.Renew();
      PX.Renew();
      return A.T;
    }));
    return A.F;
  }

  public static bool KeyForwardSlashD() {
    L = L || P1.TryEnqueue(_ => X2.E1(A.T) && P2.TryEnqueue(_ => {
      AY = Till(ci => L && (99 >= ci) && X2.YX(PX.YAxis(ci) * CY, PX.XAxis(ci) / -CY) && Time.XO(EY), AY) - 1;
      return A.T;
    }));
    return A.F;
  }

  public static bool KeyDU() {
    return P1.TryEnqueue(_ => XO(LA, UC));
  }

  public static bool KeyDD() {
    return A.T;
  }

  public static bool KeyAU() {
    return P1.TryEnqueue(_ => XO(RA, UC));
  }

  public static bool KeyAD() {
    return A.T;
  }

  public static bool OnU(uint i) => i switch {
    Key.ForwardSlash => KeyForwardSlashU(),
    Key.D => KeyDU(),
    Key.A => KeyAU(),
    _ => A.T,
  };

  public static bool OnD(uint i) => i switch {
    Key.ForwardSlash => KeyForwardSlashD(),
    Key.D => KeyDD(),
    Key.A => KeyAD(),
    _ => A.T,
  };

  public static bool XO(ushort e_1, double t) {
    X1.EE(e_1, A.T);
    Time.XO(t);
    X1.EE(e_1, A.F);
    return A.T;
  }

  public static IntPtr OnHookD2(int nCode, IntPtr wParam, IntPtr lParam) {
    IntPtr next = Native.CallNextHookEx(hookD2, nCode, wParam, lParam);
    if (nCode < 0) return next;

    uint key = (uint)Marshal.ReadInt32(lParam);

    switch ((uint)wParam) {
      case WM_SYSKEYDOWN or WM_KEYDOWN:
        OnD(key);
        return next;
      case WM_SYSKEYUP or WM_KEYUP:
        OnU(key);
        return next;
      default:
        return next;
    }
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
    Subscribe(new Native.MSG());
  }

  public static volatile Xyloid2 X2 = new(Contact.Device(args => args.Contains("RZCONTROL")));
  public static volatile Xyloid1 X1 = new(Contact.Device(args => args.Contains("RZCONTROL")));

  public static volatile Pattern PY = new("vandal");
  public static volatile Pattern PX = new("vandal");

  public static volatile Partner P2 = new(256);
  public static volatile Partner P1 = new(256);

  public static readonly double US = 209.9999;
  public static readonly double UC = 99.99999;

  public static readonly ushort RA = 0x4D;
  public static volatile bool R = A.F;

  public static readonly ushort LA = 0x4B;
  public static volatile bool L = A.F;

  public static readonly int EY = 8;
  public static readonly int CY = 2;
  public static volatile int AY = 0;
  public static volatile int AX = 0;

  public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
  public static readonly LowLevelProc onHookD2 = OnHookD2;
  public static volatile IntPtr hookD2 = IntPtr.Zero;

  public const uint WM_SYSKEYDOWN = 0x0104;
  public const uint WM_SYSKEYUP = 0x0105;
  public const uint WM_KEYDOWN = 0x0100;
  public const uint WM_KEYUP = 0x0101;
}
