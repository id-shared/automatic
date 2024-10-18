using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  public static readonly Specter S9 = new(256);
  public static readonly Specter S3 = new(256);
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
    int scale = 2;

    switch ((uint)wParam) {
      case WM_LBUTTONDOWN:
        HL = A.T;
        S1.TryEnqueue(_ => {
          I(LA);
          return S3.TryEnqueue(_ => {
            YA = Till(_ => (33 >= _) && HL && D1.Y(YAxis(_) * scale) && W(4.05 * scale), YA) - 1;
            O(LA);
            return A.T;
          });
        });
        return next;
      case WM_LBUTTONUP:
        HL = A.F;
        S1.TryEnqueue(_ => {
          O(LA);
          return S3.TryEnqueue(_ => {
            YA = YA - Till(_ => (YA >= _) && D1.Y(YAxis(_) * -scale) && W(4.05 * scale), 0);
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
      //99 => 2,
      //98 => 2,
      //97 => 2,
      //96 => 2,
      //95 => 2,
      //94 => 2,
      //93 => 2,
      //92 => 2,
      //91 => 2,
      //90 => 2,
      //89 => 2,
      //88 => 2,
      //87 => 2,
      //86 => 2,
      //85 => 2,
      //84 => 2,
      //83 => 2,
      //82 => 2,
      //81 => 2,
      //80 => 2,
      //79 => 1,
      //78 => 1,
      //77 => 1,
      //76 => 1,
      //75 => 1,
      //74 => 1,
      //73 => 1,
      //72 => 1,
      //71 => 1,
      //70 => 1,
      //69 => 1,
      //68 => 1,
      //67 => 1,
      //66 => 1,
      //65 => 1,
      //64 => 1,
      //63 => 1,
      //62 => 1,
      //61 => 1,
      //60 => 1,
      //59 => 1,
      //58 => 1,
      //57 => 1,
      //56 => 1,
      //55 => 1,
      //54 => 1,
      //53 => 1,
      //52 => 1,
      //51 => 1,
      //50 => 1,
      //49 => 1,
      //48 => 1,
      //47 => 1,
      //46 => 1,
      //45 => 1,
      //44 => 1,
      //43 => 1,
      //42 => 1,
      //41 => 1,
      //40 => 1,
      //39 => 1,
      //38 => 1,
      //37 => 1,
      //36 => 1,
      //35 => 1,
      //34 => 1,
      33 => 1,
      32 => 1,
      31 => 1,
      30 => 1,
      29 => 1,
      28 => 1,
      27 => 1,
      26 => 1,
      25 => 1,
      24 => 0,
      23 => 1,
      22 => 0,
      21 => 1,
      20 => 0,
      19 => 1,
      18 => 0,
      17 => 1,
      16 => 0,
      15 => 1,
      14 => 0,
      13 => 1,
      12 => 0,
      11 => 1,
      10 => 0,
      9 => 1,
      8 => 0,
      7 => 1,
      6 => 0,
      5 => 1,
      4 => 0,
      3 => 1,
      2 => 0,
      1 => 1,
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
