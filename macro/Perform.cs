using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  static readonly WorkerPool FX = new(1, 1024);
  static readonly uint[] ML = [KeyE.S, KeyE.C, KeyE.A];
  static readonly uint[] AR = [KeyA.R];
  static readonly uint[] AL = [KeyA.L];
  static volatile bool FLMB = A.T;
  static volatile bool FREE = A.T;
  const double TL = 99.0;
  const double IL = 19.0;

  public Perform() {
    hookX2 = SetHook(hookCallBackX2, WH_KEYBOARD_LL);
    hookX1 = SetHook(hookCallbackX1, WH_MOUSE_LL);
    Subscribe(new MSG());
  }

  static bool KeyDU() {
    FREE = A.F;
    IO(TL, AL);
    FREE = A.T;
    return A.T;
  }

  static bool KeyAU() {
    FREE = A.F;
    IO(TL, AR);
    FREE = A.T;
    return A.T;
  }

  static bool OnU(uint i) => i switch {
    KeyX.D => KeyDU(),
    KeyX.A => KeyAU(),
    _ => A.F,
  };

  static bool XO(double t, uint[] k) {
    I(k);
    O(k);
    Time.Wait(t);
    return A.F;
  }

  static bool IO(double t, uint[] k) {
    I(k);
    Time.Wait(t);
    O(k);
    return A.F;
  }

  static bool O(uint[] n) => n.All(_ => Keyboard.Input(_, A.F));

  static bool I(uint[] n) => n.All(_ => Keyboard.Input(_, A.T));

  static IntPtr HookCallbackX2(int nCode, IntPtr wParam, IntPtr lParam) {
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

  static IntPtr HookCallbackX1(int nCode, IntPtr wParam, IntPtr lParam) {
    IntPtr next = CallNextHookEx(hookX1, nCode, wParam, lParam);
    if (nCode < 0) return next;

    switch ((uint)wParam) {
      case WM_LBUTTONDOWN:
        FLMB = A.F;
        FX.TryEnqueue(() => {
          Till(_ => FREE);
          Till(_ => !FLMB && XO(IL, ML));
        });
        return next;
      case WM_LBUTTONUP:
        FLMB = A.T;
        return next;
      default:
        return next;
    }
  }

  static bool Till(Func<int, bool> z) {
    SpinWait spinner = new();
    while (!z(spinner.Count + 1)) {
      spinner.SpinOnce();
    }
    return A.T;
  }

  static void Exit() => Environment.Exit(0);

  struct Back(int code, IntPtr w, IntPtr l, IntPtr i) {
    public IntPtr wParam = w, lParam = l, iParam = i;
    public int nCode = code;
  }

  static IntPtr SetHook(Delegate proc, uint hookType) {
    using var module = Process.GetCurrentProcess().MainModule;
    if (module == null) return IntPtr.Zero;

    IntPtr handle = GetModuleHandle(module.ModuleName);
    return handle == IntPtr.Zero ? IntPtr.Zero :
      SetWindowsHookEx((int)hookType, proc, handle, 0);
  }

  static void Subscribe(MSG msg) {
    while (GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }
  }

  delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
  static readonly LowLevelProc hookCallBackX2 = HookCallbackX2;
  static readonly LowLevelProc hookCallbackX1 = HookCallbackX1;

  static volatile IntPtr hookX2 = IntPtr.Zero;
  static volatile IntPtr hookX1 = IntPtr.Zero;

  const uint WM_LBUTTONDOWN = 0x0201, WM_LBUTTONUP = 0x0202;
  const uint WM_KEYUP = 0x0101, WM_SYSKEYUP = 0x0105;
  const uint WH_KEYBOARD_LL = 13, WH_MOUSE_LL = 14;

  [StructLayout(LayoutKind.Sequential)]
  struct MSG {
    public IntPtr hWnd;
    public uint message;
    public IntPtr wParam, lParam;
    public uint time;
    public POINT pt;
  }

  [StructLayout(LayoutKind.Sequential)]
  struct POINT {
    public int x, y;
  }

  [StructLayout(LayoutKind.Sequential)]
  struct MSLLHOOKSTRUCT {
    public POINT pt;
    public uint mouseData;
    public uint flags;
    public uint time;
    public IntPtr dwExtraInfo;
  }

  [DllImport("user32.dll")]
  extern static IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  extern static bool UnhookWindowsHookEx(IntPtr hhk);

  [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
  extern static IntPtr GetModuleHandle(string lpModuleName);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  extern static bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  extern static bool TranslateMessage(ref MSG lpMsg);

  [DllImport("user32.dll")]
  extern static IntPtr DispatchMessage(ref MSG lpMsg);

  [DllImport("user32.dll")]
  extern static IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
}
