using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  private static IntPtr D2UD(Back x) {
    SD = (int)Environment.TickCount64;
    return Next(x);
  }

  private static IntPtr D2UA(Back x) {
    SA = (int)Environment.TickCount64;
    return Next(x);
  }

  private static IntPtr D2DD(Back x) {
    return Next(x);
  }

  private static IntPtr D2DA(Back x) {
    return Next(x);
  }

  private static IntPtr D1UR(Back x) {
    return Next(x);
  }

  private static IntPtr D1UL(Back x) {
    return Next(x);
  }

  private static IntPtr D1DR(Back x) {
    return Next(x);
  }

  private static IntPtr D1DL(Back x) {
    int TC64 = (int)Environment.TickCount64;
    SD = AllHeld([KeyX.D]) ? Tc64(TC64, SD, ID) : SD;
    SA = AllHeld([KeyX.A]) ? Tc64(TC64, SA, IA) : SA;
    Actor(TC64 - SD, ID, [KeyA.L]);
    Actor(TC64 - SA, IA, [KeyA.R]);
    return Next(x);
  }

  private static bool Actor(int w, int t, uint[] k) {
    return t > w && IO(t - w, k);
  }

  private static int Tc64(int w, int t, int i) {
    return (w - t) > i ? w : t;
  }

  private static volatile int SD = 0;
  private static volatile int SA = 0;
  private const int ID = 109;
  private const int IA = 109;

  private static bool Do(Action z) {
    worker.Enqueue(z);
    return A.T;
  }

  private static DedicatedWorker worker = new(1024);

  private static IntPtr OnD2U(Back x, uint i) {
    _ = KeyE.W == i && Exit();
    Unit[i] = A.F;
    return A.T switch {
      var _ when KeyX.A == i => D2UA(x),
      var _ when KeyX.D == i => D2UD(x),
      _ => Next(x),
    };
  }

  private static IntPtr OnD2D(Back x, uint i) {
    Console.WriteLine(i);
    Unit[i] = A.T;
    return A.T switch {
      var _ when KeyX.A == i => D2DA(x),
      var _ when KeyX.D == i => D2DD(x),
      _ => Next(x),
    };
  }

  private static IntPtr OnD1U(Back x, uint i) {
    Unit[i] = A.F;
    return A.T switch {
      var _ when KeyM.L == i => D1UL(x),
      var _ when KeyM.R == i => D1UR(x),
      _ => Next(x),
    };
  }

  private static IntPtr OnD1D(Back x, uint i) {
    Unit[i] = A.T;
    return A.T switch {
      var _ when KeyM.R == i => D1DR(x),
      var _ when KeyM.L == i => D1DL(x),
      _ => Next(x),
    };
  }

  private static bool ReactIO(int t, uint[] o, uint[] k) {
    return !AnyHeld(o) && IO(t, k);
  }

  private static bool ReactO(uint[] o, uint[] k) {
    return !AnyHeld(o) && O(k);
  }

  private static bool ReactI(uint[] o, uint[] k) {
    return !AnyHeld(o) && I(k);
  }

  private static bool ActIO(int t, uint[] o, uint[] k) {
    return AllHeld(o) && IO(t, k);
  }

  private static bool ActO(uint[] o, uint[] k) {
    return AllHeld(o) && O(k);
  }

  private static bool ActI(uint[] o, uint[] k) {
    return AllHeld(o) && I(k);
  }

  private static bool AnyHeld(uint[] k) {
    return k.Any(IsHeld);
  }

  private static bool AllHeld(uint[] k) {
    return k.All(IsHeld);
  }

  private static bool IsHeld(uint k) {
    return Unit.TryGetValue(k, out bool is_held) && is_held;
  }

  private static readonly Dictionary<uint, bool> Unit = [];

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

  public static IntPtr D2HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    Back x = new() {
      wParam = wParam,
      lParam = lParam,
      iParam = HookD2,
      nCode = nCode,
    };

    if (nCode >= 0) {
      uint key = (uint)Marshal.ReadInt32(lParam);
      uint act = (uint)wParam;
      return A.T switch {
        var _ when act == WM_SYSKEYDOWN => OnD2D(x, key),
        var _ when act == WM_KEYDOWN => OnD2D(x, key),
        var _ when act == WM_SYSKEYUP => OnD2U(x, key),
        var _ when act == WM_KEYUP => OnD2U(x, key),
        _ => Next(x),
      };
    } else {
      return Next(x);
    }
  }

  public static IntPtr D1HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    Back x = new() {
      wParam = wParam,
      lParam = lParam,
      iParam = HookD1,
      nCode = nCode,
    };

    if (nCode >= 0) {
      uint act = (uint)wParam;
      return A.T switch {
        var _ when act == WM_LBUTTONDOWN => OnD1D(x, KeyM.L),
        var _ when act == WM_LBUTTONUP => OnD1U(x, KeyM.L),
        var _ when act == WM_RBUTTONDOWN => OnD1D(x, KeyM.R),
        var _ when act == WM_RBUTTONUP => OnD1U(x, KeyM.R),
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
    HookD2 = SetHook(d2_hook, WH_KEYBOARD_LL);
    HookD1 = SetHook(d1_hook, WH_MOUSE_LL);

    SubscribeKey(new MSG());

    Detach(HookD2);
    Detach(HookD1);
  }

  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
  private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

  private static readonly LowLevelKeyboardProc d2_hook = D2HookCallback;
  private static readonly LowLevelMouseProc d1_hook = D1HookCallback;
  private static IntPtr HookD2 = IntPtr.Zero;
  private static IntPtr HookD1 = IntPtr.Zero;

  private const uint WH_KEYBOARD_LL = 13;
  private const uint WH_MOUSE_LL = 14;
  private const uint WM_KEYDOWN = 0x0100;
  private const uint WM_SYSKEYDOWN = 0x0104;
  private const uint WM_KEYUP = 0x0101;
  private const uint WM_SYSKEYUP = 0x0105;
  private const uint WM_LBUTTONDOWN = 0x0201;
  private const uint WM_LBUTTONUP = 0x0202;
  private const uint WM_RBUTTONDOWN = 0x0204;
  private const uint WM_RBUTTONUP = 0x0205;

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
