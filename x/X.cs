using System.Diagnostics;
using System.Runtime.InteropServices;

namespace X {
  class X {
    public bool KeyForwardSlashU() {
      key_1 = A.F;
      partner_1.TryEnqueue(_ => xyloid2.E1(A.F) && partner_2.TryEnqueue(_ => {
        a_y = Upon(ci => !key_1 && (+1 <= ci) && partner_3.TryEnqueue(_ => Pattern(ci, A.F)) && Time.XO(waiting_1 / +2), a_y) + 1;
        return A.T;
      }));
      return A.F;
    }

    public bool KeyForwardSlashD() {
      key_1 = A.T;
      _ = key_d && xyloid1.EN([Key.D], A.F);
      _ = key_a && xyloid1.EN([Key.A], A.F);
      partner_1.TryEnqueue(_ => xyloid2.E1(A.T) && partner_2.TryEnqueue(_ => {
        a_y = Till(ci => key_1 && (e_y >= ci) && partner_3.TryEnqueue(_ => Pattern(ci, A.T)) && Time.XO(waiting_1 / +1), a_y) - 1;
        return A.T;
      }));
      return A.F;
    }

    public bool KeyDU() {
      key_d = A.F;
      partner_1.TryEnqueue(_ => XO([Key.LArrow], Breakup(waiting_3)));
      return A.T;
    }

    public bool KeyDD() {
      waiting_3 = Environment.TickCount;
      key_d = A.T;
      return A.T;
    }

    public bool KeyAU() {
      key_a = A.F;
      partner_1.TryEnqueue(_ => XO([Key.RArrow], Breakup(waiting_3)));
      return A.T;
    }

    public bool KeyAD() {
      waiting_3 = Environment.TickCount;
      key_a = A.T;
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
      int dy = (a ? +1 : -1) * pattern_1.DY(e);
      int dx = (a ? -1 : +1) * pattern_1.DX(e);

      return dy == 0 && dx == 0 || xyloid2.YX(dy * c_y, dx * c_x);
    }

    public int Breakup(int e) {
      return pattern_3.DN((Environment.TickCount - e) / 10);
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

    public volatile int waiting_3 = Environment.TickCount;
    public readonly int waiting_2 = +99;
    public readonly int waiting_1 = +16;

    public readonly Pattern pattern_3 = new(+999);
    public readonly Pattern pattern_2 = new(+999);
    public readonly Pattern pattern_1 = new(+999);

    public readonly Partner partner_3 = new(+256);
    public readonly Partner partner_2 = new(+256);
    public readonly Partner partner_1 = new(+256);

    public volatile bool key_d = A.F;
    public volatile bool key_a = A.F;
    public volatile bool key_2 = A.F;
    public volatile bool key_1 = A.F;

    public readonly int e_y = +64;
    public readonly int e_x = +64;
    public readonly int c_y = +5;
    public readonly int c_x = +1;
    public volatile int a_y = +1;
    public volatile int a_x = +1;

    public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
    public volatile IntPtr hook = IntPtr.Zero;

    public const uint WM_SYSKEYDOWN = 0x0104;
    public const uint WM_SYSKEYUP = 0x0105;
    public const uint WM_KEYDOWN = 0x0100;
    public const uint WM_KEYUP = 0x0101;
  }
}
