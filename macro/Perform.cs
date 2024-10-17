using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  public static readonly Specter S9 = new(256);
  public static readonly Specter S1 = new(256);

  public static readonly Driver1 D1 = new();

  public static readonly uint[] LZ = [KeyM.L];
  public static readonly uint[] LA = [KeyE.A];

  public static readonly uint[] AR = [KeyA.R];
  public static readonly uint[] AL = [KeyA.L];

  public static readonly double XL = 209.9999;
  public static readonly double TL = 99.99999;

  public static volatile bool HR = A.F;
  public static volatile bool HL = A.F;

  public static readonly int YE = 9;
  public static volatile int YA = 0;

  public static readonly int XE = 9;
  public static volatile int XA = 0;

  public static bool KeyDU() {
    return S1.TryEnqueue(_ => IO(TL, AL));
  }

  public static bool KeyAU() {
    return S1.TryEnqueue(_ => IO(TL, AR));
  }

  public static bool OnU(uint i) => i switch {
    KeyX.D => KeyDU(),
    KeyX.A => KeyAU(),
    _ => A.F,
  };

  public static bool IO(double t, uint[] k) {
    I(k);
    Time.XO(t);
    O(k);
    return A.T;
  }

  public static bool W(double i) => Time.XO(i);

  public static bool O(uint[] k) => Driver2.Input(k, A.F);

  public static bool I(uint[] k) => Driver2.Input(k, A.T);

  public static bool H(uint[] k) => Driver2.IsHeld(k);

  public static IntPtr HookCallbackX2(int nCode, IntPtr wParam, IntPtr lParam) {
    IntPtr next = CallNextHookEx(hookX2, nCode, wParam, lParam);
    if (nCode < 0) return next;

    uint key = (uint)Marshal.ReadInt32(lParam);
    if (key == KeyE.W) Exit();

    switch ((uint)wParam) {
      case WM_SYSKEYUP or WM_KEYUP:
        OnU(key);
        return next;
      default:
        return next;
    }
  }

  public static IntPtr HookCallbackX1(int nCode, IntPtr wParam, IntPtr lParam) {
    IntPtr next = CallNextHookEx(hookX1, nCode, wParam, lParam);
    if (nCode < 0) return next;

    switch ((uint)wParam) {
      case WM_LBUTTONDOWN:
        S1.TryEnqueue(_ => {
          HL = A.T;
          I(LA);
          return S9.TryEnqueue(_ => {
            YA = Till(_ => (57 > _) && HL && D1.X(XAxis(_) * -1) && D1.Y(YAxis(_)) && W(YE), YA) - 1;
            return A.T;
          });
        });
        return next;
      case WM_LBUTTONUP:
        S1.TryEnqueue(_ => {
          HL = A.F;
          O(LA);
          return S9.TryEnqueue(_ => {
            YA = YA - Till(_ => (YA >= _) && D1.X(XAxis(_)) && D1.Y(YAxis(_) * -1) && W(YE), 0);
            return A.T;
          });
        });
        return next;
      default:
        return next;
    }
  }

  public static int YAxis(int i) {
    return i switch {
      57 => 3,
      56 => 3,
      55 => 3,
      54 => 3,
      53 => 3,
      52 => 3,
      51 => 3,
      50 => 3,
      49 => 3,
      48 => 3,
      47 => 3,
      46 => 3,
      45 => 3,
      44 => 3,
      43 => 3,
      42 => 3,
      41 => 3,
      40 => 3,
      39 => 3,
      38 => 3,
      37 => 3,
      36 => 3,
      35 => 3,
      34 => 3,
      33 => 3,
      32 => 3,
      31 => 3,
      30 => 3,
      29 => 3,
      28 => 3,
      27 => 3,
      26 => 3,
      25 => 3,
      24 => 2,
      23 => 2,
      22 => 2,
      21 => 2,
      20 => 2,
      19 => 2,
      18 => 2,
      17 => 2,
      16 => 2,
      15 => 2,
      14 => 1,
      13 => 1,
      12 => 1,
      11 => 1,
      10 => 1,
      9 => 1,
      8 => 1,
      7 => 1,
      6 => 1,
      5 => 1,
      4 => 0,
      3 => 0,
      2 => 0,
      1 => 0,
      0 => 0,
      _ => 0
    };
  }

  public static int XAxis(int i) {
    return i switch {
      57 => 3,
      56 => 3,
      55 => 3,
      54 => 3,
      53 => 3,
      52 => 3,
      51 => 3,
      50 => 3,
      49 => 3,
      48 => 3,
      47 => 3,
      46 => 3,
      45 => 3,
      44 => 3,
      43 => 3,
      42 => 3,
      41 => 3,
      40 => 3,
      39 => 3,
      38 => 3,
      37 => 3,
      36 => 3,
      35 => 3,
      34 => 3,
      33 => 3,
      32 => 3,
      31 => 3,
      30 => 3,
      29 => 3,
      28 => 3,
      27 => 3,
      26 => 3,
      25 => 3,
      24 => 2,
      23 => 2,
      22 => 2,
      21 => 2,
      20 => 2,
      19 => 2,
      18 => 2,
      17 => 2,
      16 => 2,
      15 => 2,
      14 => 1,
      13 => 1,
      12 => 1,
      11 => 1,
      10 => 1,
      9 => 1,
      8 => 1,
      7 => 1,
      6 => 1,
      5 => 1,
      4 => 0,
      3 => 0,
      2 => 0,
      1 => 0,
      0 => 0,
      _ => 0
    };
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
    hookX2 = SetHook(hookCallBackX2, WH_KEYBOARD_LL);
    hookX1 = SetHook(hookCallbackX1, WH_MOUSE_LL);
    Subscribe(new MSG());
  }

  public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
  public static readonly LowLevelProc hookCallBackX2 = HookCallbackX2;
  public static readonly LowLevelProc hookCallbackX1 = HookCallbackX1;

  public static volatile IntPtr hookX2 = IntPtr.Zero;
  public static volatile IntPtr hookX1 = IntPtr.Zero;

  public const uint WM_LBUTTONDOWN = 0x0201, WM_LBUTTONUP = 0x0202;
  public const uint WM_KEYUP = 0x0101, WM_SYSKEYUP = 0x0105;
  public const uint WH_KEYBOARD_LL = 13, WH_MOUSE_LL = 14;

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
