﻿using System.Runtime.InteropServices;
using System.Diagnostics;

class Perform {
  public static readonly Specter S2 = new(256);
  public static readonly Specter S1 = new(256);

  public static readonly Device1 D1 = new();

  public static readonly uint[] RA = [KeyA.R];
  public static bool R = A.F;

  public static readonly double LI = 209.9999;
  public static readonly double LE = 99.99999;
  public static readonly uint[] LC = [KeyM.L];
  public static readonly uint[] LA = [KeyA.L];
  public static bool L = A.F;

  public static readonly int EY = 8;
  public static readonly int CY = 2;
  public static int AY = 0;
  public static int AX = 0;

  public static bool KeyEAU() {
    L = A.F;
    S1.TryEnqueue(_ => {
      O(LC);
      return S2.TryEnqueue(_ => {
        AY = AY - Till(ci => (AY >= ci) && D1.YX(Recoil.YAxis(ci) * -CY, Recoil.XAxis(ci) / CY) && C(EY / 1.2), 00);
        //Console.WriteLine($"zz: {AY}");
        return A.T;
      });
    });
    return A.T;
  }

  public static bool KeyEAD() {
    L = A.T;
    S1.TryEnqueue(_ => {
      I(LC);
      return S2.TryEnqueue(_ => {
        AY = Till(ci => (99 >= ci) && L && D1.YX(Recoil.YAxis(ci) * CY, Recoil.XAxis(ci) / -CY) && C(EY / 1.0), 00);
        //Console.WriteLine($"aa: {AY}");
        return A.T;
      });
    });
    return A.T;
  }

  public static bool KeyDU() {
    return S1.TryEnqueue(_ => IO(LE, LA));
  }

  public static bool KeyDD() {
    return A.T;
  }

  public static bool KeyAU() {
    return S1.TryEnqueue(_ => IO(LE, RA));
  }

  public static bool KeyAD() {
    return A.T;
  }

  public static bool OnU(uint i) => i switch {
    KeyE.A => KeyEAU(),
    KeyX.D => KeyDU(),
    KeyX.A => KeyAU(),
    _ => A.F,
  };

  public static bool OnD(uint i) => i switch {
    KeyE.A => KeyEAD(),
    KeyX.D => KeyDD(),
    KeyX.A => KeyAD(),
    _ => A.F,
  };

  public static bool IX<X>(string e, X _) {
    Console.WriteLine($"{e}: {_}.");
    return A.T;
  }

  public static bool IO(double t, uint[] k) {
    I(k);
    Time.XO(t);
    O(k);
    return A.T;
  }

  public static bool O(uint[] k) => Device2.Input(k, A.F);

  public static bool I(uint[] k) => Device2.Input(k, A.T);

  public static bool H(uint[] k) => Device2.IsHeld(k);

  public static bool C(double i) => Time.XO(i);

  public static IntPtr OnHook(int nCode, IntPtr wParam, IntPtr lParam) {
    IntPtr next = CallNextHookEx(hook, nCode, wParam, lParam);
    if (nCode < 0) return next;

    uint key = (uint)Marshal.ReadInt32(lParam);

    switch ((uint)wParam) {
      case WM_SYSKEYDOWN or WM_KEYDOWN:
        if (key == KeyE.W) Exit();
        OnD(key);
        return next;
      case WM_SYSKEYUP or WM_KEYUP:
        OnU(key);
        return next;
      default:
        return next;
    }
  }

  public static int Upon(Func<int, bool> z, int i) {
    return z(i) ? Upon(z, i - 1) : i + 1;
  }

  public static int Till(Func<int, bool> z, int i) {
    return z(i) ? Till(z, i + 1) : i - 1;
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
    hook = SetHook(onHook, 13);
    Subscribe(new MSG());
  }

  public delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);
  public static readonly LowLevelProc onHook = OnHook;
  public static volatile IntPtr hook = IntPtr.Zero;

  public const uint WM_SYSKEYDOWN = 0x0104;
  public const uint WM_SYSKEYUP = 0x0105;
  public const uint WM_KEYDOWN = 0x0100;
  public const uint WM_KEYUP = 0x0101;

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

class Recoil {
  public static int YAxis(int i) {
    return i switch {
      99 => 1,
      98 => 1,
      97 => 1,
      96 => 1,
      95 => 1,
      94 => 1,
      93 => 1,
      92 => 1,
      91 => 1,
      90 => 1,
      89 => 1,
      88 => 1,
      87 => 1,
      86 => 1,
      85 => 1,
      84 => 1,
      83 => 1,
      82 => 1,
      81 => 1,
      80 => 1,
      79 => 1,
      78 => 1,
      77 => 1,
      76 => 1,
      75 => 1,
      74 => 1,
      73 => 1,
      72 => 1,
      71 => 1,
      70 => 1,
      69 => 1,
      68 => 1,
      67 => 1,
      66 => 1,
      65 => 1,
      64 => 1,
      63 => 1,
      62 => 1,
      61 => 1,
      60 => 1,
      59 => 1,
      58 => 1,
      57 => 1,
      56 => 1,
      55 => 1,
      54 => 1,
      53 => 1,
      52 => 1,
      51 => 1,
      50 => 1,
      49 => 2,
      48 => 1,
      47 => 2,
      46 => 1,
      45 => 2,
      44 => 1,
      43 => 2,
      42 => 1,
      41 => 2,
      40 => 1,
      39 => 2,
      38 => 2,
      37 => 2,
      36 => 2,
      35 => 2,
      34 => 2,
      33 => 2,
      32 => 2,
      31 => 2,
      30 => 2,
      29 => 1,
      28 => 0,
      27 => 1,
      26 => 0,
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

  public static int XAxis(int i) {
    return i switch {
      99 => 2,
      98 => 2,
      97 => 1,
      96 => 1,
      95 => 1,
      94 => 2,
      93 => 2,
      92 => 1,
      91 => 1,
      90 => 1,
      89 => 2,
      88 => 1,
      87 => 1,
      86 => 1,
      85 => 1,
      84 => 2,
      83 => 1,
      82 => 1,
      81 => 1,
      80 => 1,
      79 => 2,
      78 => 1,
      77 => 1,
      76 => 1,
      75 => 1,
      74 => 2,
      73 => 1,
      72 => 1,
      71 => 1,
      70 => 1,
      69 => 2,
      68 => 1,
      67 => 1,
      66 => 1,
      65 => 1,
      64 => 2,
      63 => 1,
      62 => 1,
      61 => 1,
      60 => 1,
      59 => 2,
      58 => 1,
      57 => 1,
      56 => 1,
      55 => 1,
      54 => 2,
      53 => 1,
      52 => 1,
      51 => 1,
      50 => 1,
      49 => 2,
      48 => 1,
      47 => 1,
      46 => 1,
      45 => 1,
      44 => 2,
      43 => 1,
      42 => 1,
      41 => 1,
      40 => 1,
      39 => 2,
      38 => 1,
      37 => 1,
      36 => 1,
      35 => 1,
      34 => 2,
      33 => 1,
      32 => 1,
      31 => 1,
      30 => 1,
      29 => 2,
      28 => 1,
      27 => 1,
      26 => 1,
      25 => 0,
      24 => 2,
      23 => 1,
      22 => 0,
      21 => 0,
      20 => 0,
      19 => 2,
      18 => 1,
      17 => 0,
      16 => 0,
      15 => 0,
      14 => 2,
      13 => 1,
      12 => 0,
      11 => 0,
      10 => 0,
      9 => 0,
      8 => 0,
      7 => 0,
      6 => 0,
      5 => 0,
      4 => 0,
      3 => 0,
      2 => 0,
      1 => 0,
      0 => 0,
      _ => 0
    };
  }
}
