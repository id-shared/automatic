using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  public static readonly Specter S9 = new(256);
  public static readonly Specter S1 = new(256);
  public static readonly Driver1 D1 = new();

  public static readonly uint[] ML = [KeyM.L];
  public static readonly uint[] EA = [KeyE.A];
  public static readonly uint[] AR = [KeyA.R];
  public static readonly uint[] AL = [KeyA.L];

  public static readonly double ZZ = (1 + Math.Sqrt(5)) / 2;
  public static readonly double XL = 209.9999;
  public static readonly double TL = 99.99999;

  public static volatile bool HR = A.F;
  public static volatile bool HL = A.F;

  public static volatile int YZ = 199;
  public static volatile int YA = 0;

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
          I(EA);

          int scale = 9;
          return Till(_ => {
            double phi = Math.Sqrt(_) / scale;
            int y = (int)phi;
            YA = YA + y;
            return HL && YZ >= YA && D1.Y(y) && W(phi);
          });
        });
        return next;
      case WM_LBUTTONUP:
        HL = A.F;
        S1.TryEnqueue(_ => {
          O(EA);

          //Thread.Sleep(100);
          //D1.Y(YA * - 1);   //Console.WriteLine(ZZ);
          YA = 0;             //Console.WriteLine(YA);
          return A.T;
        });
        return next;
      default:
        return next;
    }
  }

  public static bool Till(Func<int, bool> z) {
    SpinWait till = new();
    int i = 1;
    while (z(i)) {
      i = i + 1;
      till.SpinOnce();
    }
    return A.T;
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


//Random random = new();
//Q9.TryEnqueue(() => {
//  if (random.Next(1, 99) == 1) {
//    Console.WriteLine(_ / scale);
//  }
//});
