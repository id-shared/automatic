using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  public static readonly Specter S2 = new(256);
  public static readonly Specter S1 = new(256);

  public static readonly Device1 D1 = new();

  public static readonly uint[] RA = [KeyA.R];
  public static volatile bool R = A.F;

  public static readonly double LI = 209.9999;
  public static readonly double LE = 99.99999;
  public static readonly uint[] LA = [KeyA.L];
  public static volatile bool L = A.F;

  public static readonly int EY = 8;
  public static readonly int CY = 2;
  public static volatile int AY = 0;
  public static volatile int AX = 0;

  public static bool KeyEAU() {
    L = A.F;
    S1.TryEnqueue(_ => D1.I(2) && S2.TryEnqueue(_ => {
      Recoil recoil = new Recoil();
      AY = Upon(ci => !L && (0 <= ci) && D1.YX(recoil.YAxis(ci) * -CY, recoil.XAxis(ci) / CY) && C(EY), AY) + 1;
      return A.T;
    }));
    return L;
  }

  public static bool KeyEAD() {
    Recoil recoil = new Recoil();
    L = L || S1.TryEnqueue(_ => D1.I(1) && S2.TryEnqueue(_ => {
      AY = Till(ci => L && (99 >= ci) && D1.YX(recoil.YAxis(ci) * CY, recoil.XAxis(ci) / -CY) && C(EY), AY) - 1;
      return A.T;
    }));
    return L;
  }

  public static bool KeyDU() {
    return S1.TryEnqueue(_ => IO(LE, LA));
  }

  public static bool KeyDD() {
    return A.T;
  }

  public static bool KeyAU() {
    return S1.TryEnqueue(_ => IO(LE, RA));
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

  public static bool H(uint[] k) => Device2.IsHeld(k);

  public static bool C(double i) => Time.XO(i);

  public static IntPtr OnHook(int nCode, IntPtr wParam, IntPtr lParam) {
    IntPtr next = CallNextHookEx(hook, nCode, wParam, lParam);
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

    IntPtr handle = GetModuleHandle(module.ModuleName);
    return handle == IntPtr.Zero ? IntPtr.Zero :
      SetWindowsHookEx((int)hookType, proc, handle, 0);
  }

  public static void Subscribe(MSG msg) {
    while (GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }
  }

  public Perform() {
    hook = SetHook(onHook, 13);
    Subscribe(new MSG());
  }

  public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
  public static readonly LowLevelProc onHook = OnHook;
  public static volatile IntPtr hook = IntPtr.Zero;

  public const uint WM_SYSKEYDOWN = 0x0104;
  public const uint WM_SYSKEYUP = 0x0105;
  public const uint WM_KEYDOWN = 0x0100;
  public const uint WM_KEYUP = 0x0101;

  [StructLayout(LayoutKind.Sequential)]
  public struct MSG {
    public IntPtr hWnd;
    public uint message;
    public IntPtr wParam, lParam;
    public uint time;
    public POINT pt;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct POINT {
    public int x, y;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct MSLLHOOKSTRUCT {
    public POINT pt;
    public uint mouseData;
    public uint flags;
    public uint time;
    public IntPtr dwExtraInfo;
  }

  [DllImport("user32.dll")]
  public static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool UnhookWindowsHookEx(IntPtr hhk);

  [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
  public static extern IntPtr GetModuleHandle(string lpModuleName);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool TranslateMessage(ref MSG lpMsg);

  [DllImport("user32.dll")]
  public static extern IntPtr DispatchMessage(ref MSG lpMsg);

  [DllImport("user32.dll")]
  public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
}
