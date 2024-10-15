using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  //public static readonly WorkerPool FX = new(16, 1024);
  public static readonly uint[] ML = [KeyE.S, KeyE.C, KeyE.A];
  public static readonly uint[] AR = [KeyA.R];
  public static readonly uint[] AL = [KeyA.L];
  public static readonly double TL = 99.0;
  public static readonly double IL = 19.0;
  public static volatile bool PLMB = A.F;
  public static volatile bool HLMB = A.F;
  public static volatile bool FLMB = A.F;

  public static bool KeyDU() {
    FLMB = A.T;
    Task.Run(() => {
      IO(TL, AL);
      FLMB = A.F;
    });
    return A.T;
  }

  public static bool KeyAU() {
    FLMB = A.T;
    Task.Run(() => {
      IO(TL, AR);
      FLMB = A.F;
    });
    return A.T;
  }

  public static bool OnU(uint i) => i switch {
    KeyX.D => KeyDU(),
    KeyX.A => KeyAU(),
    _ => A.F,
  };

  public static bool XO(double t, uint[] k) {
    I(k);
    O(k);
    Time.O(t);
    return A.T;
  }

  public static bool IO(double t, uint[] k) {
    I(k);
    Time.O(t);
    O(k);
    return A.T;
  }

  public static bool O(uint[] k) => Keyboard.Input(k, A.F);

  public static bool I(uint[] k) => Keyboard.Input(k, A.T);

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
        PLMB = A.T;
        HLMB = A.T;
        Task.Run(() => {
          Till(_ => FLMB);
          Till(_ => XO(IL, ML) && HLMB);
        });
        return next;
      case WM_LBUTTONUP:
        PLMB = A.F;
        Task.Run(() => {
          Time.O(IL);
          HLMB = A.F;
        });
        return next;
      default:
        return next;
    }
  }

  public static bool Till(Func<int, bool> z) {
    while (z(1)) {
      Time.O(1);
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
