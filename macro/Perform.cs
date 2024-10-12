using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  private static volatile int UD, UA, DD, DA;
  private static readonly uint[] KR = [KeyA.R];
  private static readonly uint[] KL = [KeyA.L];
  private const int ID = 119, IA = 119;

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

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsD() => (UD - DD) < 0;

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static bool IsA() => (UA - DA) < 0;

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

  private static bool Wait(int i) {
    SpinWait.SpinUntil(() => false, i);
    return A.T;
  }

  private static void Exit() => Environment.Exit(0);

  public static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode < 0) return Next(new Back(nCode, wParam, lParam));

    uint key = (uint)Marshal.ReadInt32(lParam);
    uint act = (uint)wParam;

    if (key == KeyE.W) Exit();

    return act switch {
      WM_SYSKEYDOWN or WM_KEYDOWN => OnD(new Back(nCode, wParam, lParam), key),
      WM_SYSKEYUP or WM_KEYUP => OnU(new Back(nCode, wParam, lParam), key),
      _ => Next(new Back(nCode, wParam, lParam)),
    };
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private static IntPtr Next(Back x) => CallNextHookEx(x.iParam, x.nCode, x.wParam, x.lParam);

  private struct Back {
    public IntPtr wParam, lParam, iParam;
    public int nCode;

    public Back(int code, IntPtr w, IntPtr l) {
      nCode = code; wParam = w; lParam = l;
      iParam = Hook;
    }
  }

  private static IntPtr SetHook(Delegate proc, uint hookType) {
    using var module = Process.GetCurrentProcess().MainModule;
    if (module == null) return IntPtr.Zero;

    IntPtr handle = GetModuleHandle(module.ModuleName);
    return handle == IntPtr.Zero ? IntPtr.Zero :
      SetWindowsHookEx((int)hookType, proc, handle, 0);
  }

  private static void SubscribeKey(MSG msg) {
    while (GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }
  }

  private static bool Detach(nint id) => id != IntPtr.Zero && UnhookWindowsHookEx(id);

  public Perform() {
    Hook = SetHook(HookCallBack, WH_KEYBOARD_LL);
    SubscribeKey(new MSG());
    Detach(Hook);
  }

  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
  private static readonly LowLevelKeyboardProc HookCallBack = HookCallback;
  private static volatile IntPtr Hook = IntPtr.Zero;

  private const uint WH_KEYBOARD_LL = 13;
  private const uint WM_KEYDOWN = 0x0100, WM_SYSKEYDOWN = 0x0104;
  private const uint WM_KEYUP = 0x0101, WM_SYSKEYUP = 0x0105;

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
