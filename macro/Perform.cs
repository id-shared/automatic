using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  private static readonly WorkerPool FX = new(1, 1024);
  private static readonly uint[] ML = [KeyE.C, KeyE.A];
  private static readonly uint[] AR = [KeyA.R];
  private static readonly uint[] AL = [KeyA.L];
  private static volatile bool FLMB = A.T;
  private static volatile bool FREE = A.T;
  private const int TL = 99;

  private static bool KeyDU() {
    FREE = A.F;
    return FX.TryEnqueue(() => {
      IO(TL, AL);
      FREE = A.T;
    });
  }

  private static bool KeyAU() {
    FREE = A.F;
    return FX.TryEnqueue(() => {
      IO(TL, AR);
      FREE = A.T;
    });
  }

  private static bool OnU(uint i) => i switch {
    KeyX.D => KeyDU(),
    KeyX.A => KeyAU(),
    _ => A.F,
  };

  private static bool XO(int t1, int t, uint[] k) {
    I(k);
    Wait(() => A.F, t);
    O(k);
    Wait(() => A.F, t1);
    return A.T;
  }

  private static bool IO(int t, uint[] k) {
    I(k);
    Wait(() => A.F, t);
    O(k);
    return A.T;
  }

  private static bool O(uint[] n) => n.All(_ => Keyboard.Input(_, A.F));

  private static bool I(uint[] n) => n.All(_ => Keyboard.Input(_, A.T));

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
        FX.TryEnqueue(() => {
          Till(_ => FREE);
          Till(_ => XO(15, 1, ML) && FLMB);
        });
        return next;
      case WM_LBUTTONUP:
        FLMB = A.T;
        return next;
      default:
        return next;
    }
  }

  private static bool Wait(Func<bool> z, int i) {
    return SpinWait.SpinUntil(z, i);
  }

  private static bool Till(Func<int, bool> z) {
    SpinWait spinner = new();
    while (!z(spinner.Count + 1)) {
      spinner.SpinOnce();
    }
    return A.T;
  }

  private static void Exit() => Environment.Exit(0);

  private struct Back(int code, IntPtr w, IntPtr l, IntPtr i) {
    public IntPtr wParam = w, lParam = l, iParam = i;
    public int nCode = code;
  }

  private static IntPtr SetHook(Delegate proc, uint hookType) {
    using var module = Process.GetCurrentProcess().MainModule;
    if (module == null) return IntPtr.Zero;

    IntPtr handle = GetModuleHandle(module.ModuleName);
    return handle == IntPtr.Zero ? IntPtr.Zero :
      SetWindowsHookEx((int)hookType, proc, handle, 0);
  }

  private static void Subscribe(MSG msg) {
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

  private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
  private static readonly LowLevelProc hookCallBackX2 = HookCallbackX2;
  private static readonly LowLevelProc hookCallbackX1 = HookCallbackX1;

  private static volatile IntPtr hookX2 = IntPtr.Zero;
  private static volatile IntPtr hookX1 = IntPtr.Zero;

  private const uint WM_LBUTTONDOWN = 0x0201, WM_LBUTTONUP = 0x0202;
  private const uint WM_KEYUP = 0x0101, WM_SYSKEYUP = 0x0105;
  private const uint WH_KEYBOARD_LL = 13, WH_MOUSE_LL = 14;

  [StructLayout(LayoutKind.Sequential)]
  private struct MSG {
    public IntPtr hWnd;
    public uint message;
    public IntPtr wParam, lParam;
    public uint time;
    public POINT pt;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct POINT {
    public int x, y;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct MSLLHOOKSTRUCT {
    public POINT pt;
    public uint mouseData;
    public uint flags;
    public uint time;
    public IntPtr dwExtraInfo;
  }

  [DllImport("user32.dll")]
  private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool UnhookWindowsHookEx(IntPtr hhk);

  [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
  private static extern IntPtr GetModuleHandle(string lpModuleName);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool TranslateMessage(ref MSG lpMsg);

  [DllImport("user32.dll")]
  private static extern IntPtr DispatchMessage(ref MSG lpMsg);

  [DllImport("user32.dll")]
  private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
}
