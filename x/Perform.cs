using System.Diagnostics;
using System.Runtime.InteropServices;

class Perform {
  public volatile Xyloid2 X2 = new(Contact.Device(args => args.Contains("RZCONTROL")));
  public volatile Xyloid1 X1 = new(Contact.Device(args => args.Contains("RZCONTROL")));

  public volatile Pattern PY = new("vandal"); // TODO: use
  public volatile Pattern PX = new("vandal");

  public volatile Partner P2 = new(256);
  public volatile Partner P1 = new(256);

  public readonly uint[] RA = [KeyA.R];
  public volatile bool R = A.F;

  public readonly double LE = 209.9999;
  public readonly double LC = 99.99999;
  public readonly uint[] LA = [KeyA.L];
  public volatile bool L = A.F;

  public readonly int EY = 8;
  public readonly int CY = 2;
  public volatile int AY = 0;
  public volatile int AX = 0;

  public bool KeyEAU() {
    L = A.F;
    P1.TryEnqueue(_ => X2.E1(A.F) && P2.TryEnqueue(_ => {
      AY = Upon(ci => !L && (0 <= ci) && X2.YX(PX.YAxis(ci) * -CY, PX.XAxis(ci) / CY, A.F) && Time.XO(EY), AY) + 1;
      PX = new Pattern("vandal");
      return A.T;
    }));
    return L;
  }

  public bool KeyEAD() {
    L = L || P1.TryEnqueue(_ => X2.E1(A.T) && P2.TryEnqueue(_ => {
      AY = Till(ci => L && (99 >= ci) && X2.YX(PX.YAxis(ci) * CY, PX.XAxis(ci) / -CY, A.T) && Time.XO(EY), AY) - 1;
      return A.T;
    }));
    return L;
  }

  public bool KeyDU() {
    return P1.TryEnqueue(_ => IO(LC, LA));
  }

  public bool KeyDD() {
    return A.T;
  }

  public bool KeyAU() {
    return P1.TryEnqueue(_ => IO(LC, RA));
  }

  public bool KeyAD() {
    return A.T;
  }

  public bool OnU(uint i) => i switch {
    KeyE.A => KeyEAU(),
    KeyX.D => KeyDU(),
    KeyX.A => KeyAU(),
    _ => A.F,
  };

  public bool OnD(uint i) => i switch {
    KeyE.A => KeyEAD(),
    KeyX.D => KeyDD(),
    KeyX.A => KeyAD(),
    _ => A.T,
  };

  public bool IX<X>(string e, X _) {
    Console.WriteLine($"{e}: {_}.");
    return A.T;
  }

  public bool IO(double t, uint[] k) {
    X1.EE(k, A.T);
    Time.XO(t);
    X1.EE(k, A.F);
    return A.T;
  }

  public IntPtr OnHookD1(int nCode, IntPtr wParam, IntPtr lParam) {
    IntPtr next = Native.CallNextHookEx(hookD1, nCode, wParam, lParam);
    if (nCode < 0) return next;

    uint key = (uint)Marshal.ReadInt32(lParam);

    switch ((uint)wParam) {
      case WM_SYSKEYDOWN or WM_KEYDOWN:
        //if (key == KeyE.W) Exit();
        OnD(key);
        return next;
      case WM_SYSKEYUP or WM_KEYUP:
        OnU(key);
        return next;
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

  public Perform() {
    hookD1 = SetHook(OnHookD1, 13);
    Subscribe(new Native.MSG());
  }

  public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
  public volatile IntPtr hookD1 = IntPtr.Zero;

  public const uint WM_SYSKEYDOWN = 0x0104;
  public const uint WM_SYSKEYUP = 0x0105;
  public const uint WM_KEYDOWN = 0x0100;
  public const uint WM_KEYUP = 0x0101;
}
