using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  private static readonly uint[] KR = { KeyA.R };
  private static readonly uint[] KL = { KeyA.L };
  private static volatile bool LT = A.T;
  private const int IT = 119;

  private static IntPtr KeyDU(Back x) {
    IntPtr next = Next(x);
    LT = A.F;
    IO(IT, KL);
    LT = A.T;
    return next;
  }

  private static IntPtr KeyDD(Back x) {
    return Next(x);
  }

  private static IntPtr KeyAU(Back x) {
    IntPtr next = Next(x);
    LT = A.F;
    IO(IT, KR);
    LT = A.T;
    return next;
  }

  private static IntPtr KeyAD(Back x) {
    return Next(x);
  }

  private static IntPtr OnU(Back x, uint i) => i switch {
    KeyX.D => KeyDU(x),
    KeyX.A => KeyAU(x),
    _ => Next(x),
  };

  private static IntPtr OnD(Back x, uint i) => i switch {
    KeyX.D => KeyDD(x),
    KeyX.A => KeyAD(x),
    _ => Next(x),
  };

  private static bool IO(int t, uint[] k) {
    I(k);
    Wait(t);
    O(k);
    return A.T;
  }

  private static bool O(uint[] n) => n.All(_ => Keyboard.Input(_, A.F));

  private static bool I(uint[] n) => n.All(_ => Keyboard.Input(_, A.T));

  public static IntPtr HookCallbackX2(int nCode, IntPtr wParam, IntPtr lParam) {
    Back back = new(nCode, wParam, lParam, hookX2);
    if (nCode < 0) return Next(back);

    uint key = (uint)Marshal.ReadInt32(lParam);
    if (key == KeyE.W) Exit();

    switch ((uint)wParam) {
      case WM_SYSKEYDOWN or WM_KEYDOWN:
        return OnD(back, key);
      case WM_SYSKEYUP or WM_KEYUP:
        return OnU(back, key);
      default:
        return Next(back);
    }
  }

  public static IntPtr HookCallbackX1(int nCode, IntPtr wParam, IntPtr lParam) {
    Back back = new(nCode, wParam, lParam, hookX1);
    if (nCode < 0) return Next(back);

    switch ((uint)wParam) {
      case WM_LBUTTONDOWN:
        SpinWait.SpinUntil(() => LT, IT);
        return Next(back);
      default:
        return Next(back);
    }
  }

  private static IntPtr Next(Back x) => CallNextHookEx(x.iParam, x.nCode, x.wParam, x.lParam);

  private static bool Wait(int i) {
    SpinWait.SpinUntil(() => A.F, i);
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

  private static bool Detach(IntPtr id) => id != IntPtr.Zero && UnhookWindowsHookEx(id);

  public Perform() {
    hookX2 = SetHook(hookCallBackX2, WH_KEYBOARD_LL);
    hookX1 = SetHook(hookCallbackX1, WH_MOUSE_LL);
    Subscribe(new MSG());
    Detach(hookX2);
    Detach(hookX1);
  }

  private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
  private static readonly LowLevelProc hookCallBackX2 = HookCallbackX2;
  private static readonly LowLevelProc hookCallbackX1 = HookCallbackX1;

  private static volatile IntPtr hookX2 = IntPtr.Zero;
  private static volatile IntPtr hookX1 = IntPtr.Zero;

  private const uint WM_MOUSEMOVE = 0x0200, WM_LBUTTONDOWN = 0x0201, WM_LBUTTONUP = 0x0202, WM_RBUTTONDOWN = 0x0204, WM_RBUTTONUP = 0x0205;
  private const uint WM_KEYDOWN = 0x0100, WM_KEYUP = 0x0101, WM_SYSKEYDOWN = 0x0104, WM_SYSKEYUP = 0x0105;
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
