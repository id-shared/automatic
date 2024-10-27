using System.Diagnostics;
using System.Runtime.InteropServices;

class Perform {
  public bool KeyForwardSlashU() {
    L = A.F;
    P1.TryEnqueue(_ => X2.E1(A.F) && P2.TryEnqueue(_ => {
      AY = Upon(ci => !L && (0 <= ci) && X2.YX(PY.YAxis(ci) * -CY, PX.XAxis(ci) * CX) && Time.XO(+1), AY) + 1;
      return A.T;
    }));
    return A.F;
  }

  public bool KeyForwardSlashD() {
    L = L || P1.TryEnqueue(_ => X2.E1(A.T) && P2.TryEnqueue(_ => {
      AY = Till(ci => L && (99 >= ci) && X2.YX(PX.YAxis(ci) * CY, PX.XAxis(ci) * -CX) && Time.XO(EY), AY) - 1;
      return A.T;
    }));
    return A.F;
  }

  public bool KeyDU() {
    return P1.TryEnqueue(_ => XO(LA, UC));
  }

  public bool KeyDD() {
    return A.T;
  }

  public bool KeyAU() {
    return P1.TryEnqueue(_ => XO(RA, UC));
  }

  public bool KeyAD() {
    return A.T;
  }

  public bool OnU(uint i) => i switch {
    Key.ForwardSlash => KeyForwardSlashU(),
    Key.D => KeyDU(),
    Key.A => KeyAU(),
    _ => A.T,
  };

  public bool OnD(uint i) => i switch {
    Key.ForwardSlash => KeyForwardSlashD(),
    Key.D => KeyDD(),
    Key.A => KeyAD(),
    _ => A.T,
  };

  public bool XO(ushort e_1, double t) {
    X1.EE(e_1, A.T);
    Time.XO(t);
    X1.EE(e_1, A.F);
    return A.T;
  }

  public IntPtr OnHookD2(int nCode, IntPtr wParam, IntPtr lParam) {
    IntPtr next = Native.CallNextHookEx(hookD2, nCode, wParam, lParam);
    if (nCode < 0) return next;

    uint key = (uint)Marshal.ReadInt32(lParam);

    switch ((uint)wParam) {
      case WM_SYSKEYDOWN or WM_KEYDOWN:
        return OnD(key) ? next : 1;
      case WM_SYSKEYUP or WM_KEYUP:
        return OnU(key) ? next : 1;
      default:
        return next;
    }
  }

  public int Upon(Func<int, bool> z, int i) {
    return z(i) ? Upon(z, i - 1) : i;
  }

  public int Till(Func<int, bool> z, int i) {
    return z(i) ? Till(z, i + 1) : i;
  }

  public void Exit() => Environment.Exit(0);

  public struct Back(int code, IntPtr w, IntPtr l, IntPtr i) {
    public IntPtr wParam = w, lParam = l, iParam = i;
    public int nCode = code;
  }

  public IntPtr SetHook(Delegate proc, uint hookType) {
    using var module = Process.GetCurrentProcess().MainModule;
    if (module == null) return IntPtr.Zero;

    IntPtr handle = Native.GetModuleHandle(module.ModuleName);
    return handle == IntPtr.Zero ? IntPtr.Zero :
      Native.SetWindowsHookEx((int)hookType, proc, handle, 0);
  }

  public void Subscribe(Native.MSG msg) {
    while (Native.GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      Native.TranslateMessage(ref msg);
      Native.DispatchMessage(ref msg);
    }
  }

  public Perform(string c) {
    X = new(c);
    X1 = new(X);
    X2 = new(X);
    hookD2 = SetHook(new LowLevelProc(OnHookD2), 13);
    Subscribe(new Native.MSG());
  }

  public Xyloid2 X2;
  public Xyloid1 X1;
  public Xyloid X;

  public volatile Pattern PY = new(999);
  public volatile Pattern PX = new(999);

  public volatile Partner P2 = new(256);
  public volatile Partner P1 = new(256);

  public readonly double US = 209.9999;
  public readonly double UC = 99.99999;

  public readonly ushort RA = 0x4D;
  public volatile bool R = A.F;

  public readonly ushort LA = 0x4B;
  public volatile bool L = A.F;

  public readonly int EY = 12;
  public readonly int CY = 5;
  public readonly int CX = 1;
  public volatile int AY = 0;
  public volatile int AX = 0;

  public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
  public volatile IntPtr hookD2 = IntPtr.Zero;

  public const uint WM_SYSKEYDOWN = 0x0104;
  public const uint WM_SYSKEYUP = 0x0105;
  public const uint WM_KEYDOWN = 0x0100;
  public const uint WM_KEYUP = 0x0101;
}
