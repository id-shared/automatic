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
        HL = A.T;
        S1.TryEnqueue(_ => {
          I(LA);
          return S9.TryEnqueue(_ => {
            YA = Till(_ => (59 > _) && HL && D1.Y(YAxis(_)) && W(YE), YA) - 1;
            return A.T;
          });
        });
        return next;
      case WM_LBUTTONUP:
        HL = A.F;
        S1.TryEnqueue(_ => {
          O(LA);
          return S9.TryEnqueue(_ => {
            YA = YA - Till(_ => (YA >= _) && D1.Y(YAxis(_) * -1) && W(YE), 0);
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
      59 => 6,
      58 => 6,
      57 => 6,
      56 => 6,
      55 => 6,
      54 => 6,
      53 => 6,
      52 => 6,
      51 => 6,
      50 => 6,
      49 => 5,
      48 => 5,
      47 => 5,
      46 => 5,
      45 => 5,
      44 => 5,
      43 => 5,
      42 => 5,
      41 => 5,
      40 => 5,
      39 => 4,
      38 => 4,
      37 => 4,
      36 => 4,
      35 => 4,
      34 => 4,
      33 => 4,
      32 => 4,
      31 => 4,
      30 => 4,
      29 => 3,
      28 => 3,
      27 => 3,
      26 => 3,
      25 => 3,
      24 => 3,
      23 => 3,
      22 => 3,
      21 => 3,
      20 => 3,
      19 => 2,
      18 => 2,
      17 => 2,
      16 => 2,
      15 => 2,
      14 => 2,
      13 => 2,
      12 => 2,
      11 => 2,
      10 => 2,
      9 => 1,
      8 => 1,
      7 => 1,
      6 => 1,
      5 => 1,
      4 => 1,
      3 => 1,
      2 => 1,
      1 => 1,
      0 => 1,
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
