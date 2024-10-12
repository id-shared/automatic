using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  private static IntPtr KeyDU(Back x) {
    IntPtr next = Next(x);
    UD = (int)Environment.TickCount64;
    IO(ID, KL);
    return next;
  }

  private static IntPtr KeyDD(Back x) {
    IntPtr next = Next(x);
    DD = (int)Environment.TickCount64;
    return next;
  }

  private static IntPtr KeyAU(Back x) {
    IntPtr next = Next(x);
    UA = (int)Environment.TickCount64;
    IO(IA, KR);
    return next;
  }

  private static IntPtr KeyAD(Back x) {
    IntPtr next = Next(x);
    DA = (int)Environment.TickCount64;
    return next;
  }

  private static bool IsD() {
    return 0 > (UD - DD);
  }

  private static bool IsA() {
    return 0 > (UA - DA);
  }

  private static readonly uint[] KR = [KeyA.R];
  private static readonly uint[] KL = [KeyA.L];
  private static volatile int UD = 0;
  private static volatile int UA = 0;
  private static volatile int DD = 0;
  private static volatile int DA = 0;
  private const int ID = 109;
  private const int IA = 109;

  private static IntPtr OnU(Back x, uint i) {
    return A.T switch {
      var _ when KeyX.D == i => KeyDU(x),
      var _ when KeyX.A == i => KeyAU(x),
      _ => Next(x),
    };
  }

  private static IntPtr OnD(Back x, uint i) {
    _ = KeyE.W == i && Exit();
    return A.T switch {
      var _ when KeyX.D == i => KeyDD(x),
      var _ when KeyX.A == i => KeyAD(x),
      _ => Next(x),
    };
  }

  private static bool IO(int t, uint[] k) {
    I(k);
    Wait(t);
    O(k);
    return A.T;
  }

  private static bool O(uint[] n) {
    return n.All(_ => Keyboard.Input(_, A.F));
  }

  private static bool I(uint[] n) {
    return n.All(_ => Keyboard.Input(_, A.T));
  }

  private static bool Wait(int i) {
    Thread.Sleep(i);
    return A.T;
  }

  private static bool Exit() {
    Environment.Exit(0);
    return A.T;
  }

  public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    Back x = new() {
      wParam = wParam,
      lParam = lParam,
      iParam = Hook,
      nCode = nCode,
    };

    if (nCode >= 0) {
      uint key = (uint)Marshal.ReadInt32(lParam);
      uint act = (uint)wParam;
      return A.T switch {
        var _ when act == WM_SYSKEYDOWN => OnD(x, key),
        var _ when act == WM_KEYDOWN => OnD(x, key),
        var _ when act == WM_SYSKEYUP => OnU(x, key),
        var _ when act == WM_KEYUP => OnU(x, key),
        _ => Next(x),
      };
    } else {
      return Next(x);
    }
  }

  private static IntPtr Next(Back x) {
    return CallNextHookEx(x.iParam, x.nCode, x.wParam, x.lParam);
  }

  private struct Back {
    public IntPtr wParam { get; set; }
    public IntPtr lParam { get; set; }
    public IntPtr iParam { get; set; }
    public int nCode { get; set; }
  }

  private static IntPtr Do(Action z, Back x) {
    worker.Enqueue(z);
    return Next(x);
  }

  private static DedicatedWorker worker = new(1024);

  private static IntPtr SetHook(Delegate proc, uint hookType) {
    using ProcessModule? module = Process.GetCurrentProcess().MainModule;

    switch (A.T) {
      case var _ when module == null:
        return IntPtr.Zero;
      default:
        IntPtr handle = GetModuleHandle(module.ModuleName);

        return A.T switch {
          var _ when handle == IntPtr.Zero => IntPtr.Zero,
          _ => SetWindowsHookEx((int)hookType, proc, handle, 0),
        };
    }
  }

  private static void SubscribeKey(MSG msg) {
    while (GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }
  }

  private static bool Detach(nint id) {
    return A.T switch {
      var _ when id == IntPtr.Zero => A.F,
      _ => UnhookWindowsHookEx(id),
    };
  }

  public Perform() {
    Hook = SetHook(HookCallBack, WH_KEYBOARD_LL);
    SubscribeKey(new MSG());
    Detach(Hook);
  }

  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
  private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

  private static readonly LowLevelKeyboardProc HookCallBack = HookCallback;
  private static volatile IntPtr Hook = IntPtr.Zero;

  private const uint WH_KEYBOARD_LL = 13;
  private const uint WM_KEYDOWN = 0x0100;
  private const uint WM_SYSKEYDOWN = 0x0104;
  private const uint WM_KEYUP = 0x0101;
  private const uint WM_SYSKEYUP = 0x0105;

  [StructLayout(LayoutKind.Sequential)]
  private struct MSG {
    public IntPtr hWnd;
    public uint message;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint time;
    public POINT pt;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct POINT {
    public int x;
    public int y;
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
