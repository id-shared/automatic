using System.Diagnostics;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;

class Perform {
  public bool KeyForwardSlashU() {
    L = A.F;
    P1.TryEnqueue(_ => X2.E1(A.F) && P2.TryEnqueue(_ => {
      AY = Upon(ci => !L && (1 <= ci) && P3.TryEnqueue(_ => PatternD(ci, A.F)) && Time.XO(T1), AY) + 1;
      return A.T;
    }));
    return A.F;
  }

  public bool KeyForwardSlashD() {
    L = A.T;
    P1.TryEnqueue(_ => X2.E1(A.T) && P2.TryEnqueue(_ => {
      AY = Till(ci => L && (EY >= ci) && P3.TryEnqueue(_ => PatternD(ci, A.T)) && Time.XO(T2), AY) - 1;
      X2.E1(A.F);
      return A.T;
    }));
    return A.F;
  }

  public bool PatternD(int e, bool a) {
    int dy = (a ? +1 : -1) * PX.DY(e);
    int dx = (a ? -1 : +1) * PX.DX(e);

    return dy == 0 && dx == 0 || X2.YX(dy, dx);
  }

  public bool KeyDU() {
    return P1.TryEnqueue(_ => XO(LA, T3));
  }

  public bool KeyDD() {
    return A.T;
  }

  public bool KeyAU() {
    return P1.TryEnqueue(_ => XO(RA, T3));
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
        _ = (uint)ConsoleKey.LeftWindows == key && Exit();
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

  public bool Exit() {
    Environment.Exit(0);
    return A.T;
  }

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

  public readonly Pattern PZ = new(999);
  public readonly Pattern PY = new(999);
  public readonly Pattern PX = new(999);

  public readonly Partner P3 = new(256);
  public readonly Partner P2 = new(256);
  public readonly Partner P1 = new(256);

  public readonly double T4 = 209.9999;
  public readonly double T3 = 99.99999;
  public readonly double T2 = 15.99999;
  public readonly double T1 = 1.999999;

  public readonly ushort RA = 0x4D;
  public volatile bool R = A.F;

  public readonly ushort LA = 0x4B;
  public volatile bool L = A.F;

  public readonly int EY = 10;
  public readonly int EX = 64;
  public volatile int AY = 1;
  public volatile int AX = 1;

  public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
  public volatile IntPtr hookD2 = IntPtr.Zero;

  public const uint WM_SYSKEYDOWN = 0x0104;
  public const uint WM_SYSKEYUP = 0x0105;
  public const uint WM_KEYDOWN = 0x0100;
  public const uint WM_KEYUP = 0x0101;
}
