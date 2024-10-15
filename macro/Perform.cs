using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  public static readonly WorkerPool FX = new(16, 1024);
  public static readonly uint[] ML = [KeyE.C, KeyE.A];
  public static readonly uint[] AR = [KeyA.R];
  public static readonly uint[] AL = [KeyA.L];
  public static readonly double TL = 99.0;
  public static readonly double IL = 99.0;
  public static volatile bool FLMB = A.T;
  public static volatile bool FREE = A.T;

  public static bool KeyDU() {
    FREE = A.F;
    return FX.TryEnqueue(() => {
      IO(TL, AL);
      FREE = A.T;
    });
  }

  public static bool KeyAU() {
    FREE = A.F;
    return FX.TryEnqueue(() => {
      IO(TL, AR);
      FREE = A.T;
    });
  }

  public static bool OnU(uint i) => i switch {
    KeyX.D => KeyDU(),
    KeyX.A => KeyAU(),
    _ => A.F,
  };

  public static bool XO(double t, uint[] k) {
    I(k);
    O(k);
    Time.Wait(t);
    return A.F;
  }

  public static bool IO(double t, uint[] k) {
    I(k);
    Time.Wait(t);
    O(k);
    return A.F;
  }

  public static bool O(uint[] n) => n.All(_ => Keyboard.Input(_, A.F));

  public static bool I(uint[] n) => n.All(_ => Keyboard.Input(_, A.T));

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
        FLMB = A.F;
        Task.Run(() => {
          Till(_ => FREE);
          Till(_ => !FLMB && IO(IL, ML));
        });
        return next;
      case WM_LBUTTONUP:
        FLMB = A.T;
        return next;
      default:
        return next;
    }
  }

  public static bool Till(Func<int, bool> z) {
    SpinWait spinner = new();
    while (!z(spinner.Count + 1)) {
      spinner.SpinOnce();
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
