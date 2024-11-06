using System.Diagnostics;
using System.Runtime.InteropServices;

namespace X {
  class X {
    public bool KeyForwardSlashU() {
      K1 = A.F;
      P1.TryEnqueue(_ => xyloid2.E1(A.F) && P2.TryEnqueue(_ => {
        AY = Upon(ci => !K1 && (1 <= ci) && P3.TryEnqueue(_ => Pattern(ci, A.F)) && Time.XO(T1 / +2), AY) + 1;
        return A.T;
      }));
      return A.F;
    }

    public bool KeyForwardSlashD() {
      K1 = A.T;
      _ = KD && xyloid1.EN([Key.D], A.F);
      _ = KA && xyloid1.EN([Key.A], A.F);
      P1.TryEnqueue(_ => xyloid2.E1(A.T) && P2.TryEnqueue(_ => {
        AY = Till(ci => K1 && (EY >= ci) && P3.TryEnqueue(_ => Pattern(ci, A.T)) && Time.XO(T1 / +1), AY) - 1;
        return A.T;
      }));
      return A.F;
    }

    public bool KeyDU() {
      KD = A.F;
      P1.TryEnqueue(_ => XO([Key.LArrow], Breakup(T9)));
      return A.T;
    }

    public bool KeyDD() {
      T9 = Environment.TickCount;
      KD = A.T;
      return A.T;
    }

    public bool KeyAU() {
      KA = A.F;
      P1.TryEnqueue(_ => XO([Key.RArrow], Breakup(T9)));
      return A.T;
    }

    public bool KeyAD() {
      T9 = Environment.TickCount;
      KA = A.T;
      return A.T;
    }

    public bool OnU(uint i) => i switch {
      Key.FSlash => KeyForwardSlashU(),
      Key.D => KeyDU(),
      Key.A => KeyAU(),
      _ => A.T,
    };

    public bool OnD(uint i) => i switch {
      Key.FSlash => KeyForwardSlashD(),
      Key.D => KeyDD(),
      Key.A => KeyAD(),
      _ => A.T,
    };

    public bool Pattern(int e, bool a) {
      int dy = (a ? +1 : -1) * PX.DY(e);
      int dx = (a ? -1 : +1) * PX.DX(e);

      return dy == 0 && dx == 0 || xyloid2.YX(dy * CY, dx * CX);
    }

    public int Breakup(int e) {
      return PZ.DN((Environment.TickCount - e) / 10);
    }

    public bool XO(uint[] e_1, double t) {
      xyloid1.EN(e_1, A.T);
      Time.XO(t);
      xyloid1.EN(e_1, A.F);
      return A.T;
    }

    public IntPtr OnHook(int nCode, IntPtr wParam, IntPtr lParam) {
      IntPtr next = Native.CallNextHookEx(hook, nCode, wParam, lParam);
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

    public X(string c) {
      xyloid = new(c);
      xyloid1 = new(xyloid);
      xyloid2 = new(xyloid);
      hook = SetHook(new LowLevelProc(OnHook), 13);
      Subscribe(new Native.MSG());
    }

    public static void Main() {
      try {
        X _ = new(Contact.Device(args => args.Contains("RZCONTROL")));
      } catch (Exception ex) {
        Console.WriteLine($"Error: {ex.Message}");
      } finally {
        Console.ReadKey();
      }
    }

    public Xyloid2 xyloid2;
    public Xyloid1 xyloid1;
    public Xyloid xyloid;

    public volatile int T9 = Environment.TickCount;
    public readonly int T3 = 99;
    public readonly int T1 = 16;

    public readonly Pattern PZ = new(999);
    public readonly Pattern PY = new(999);
    public readonly Pattern PX = new(999);

    public readonly Partner P3 = new(256);
    public readonly Partner P2 = new(256);
    public readonly Partner P1 = new(256);

    public volatile bool KD = A.F;
    public volatile bool KA = A.F;
    public volatile bool K2 = A.F;
    public volatile bool K1 = A.F;

    public readonly int EY = 64;
    public readonly int EX = 64;
    public readonly int CY = 5;
    public readonly int CX = 1;
    public volatile int AY = 1;
    public volatile int AX = 1;

    public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
    public volatile IntPtr hook = IntPtr.Zero;

    public const uint WM_SYSKEYDOWN = 0x0104;
    public const uint WM_SYSKEYUP = 0x0105;
    public const uint WM_KEYDOWN = 0x0100;
    public const uint WM_KEYUP = 0x0101;
  }
}
