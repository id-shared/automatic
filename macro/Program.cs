﻿using System.Runtime.InteropServices;
using System.Diagnostics;

class Program {
  public static bool D2UW() {
    Acted(KeyA.U, KeyA.U);
    Acted(KeyA.D, KeyA.D);
    return T;
  }

  public static bool D2US() {
    Acted(KeyA.U, KeyA.U);
    Acted(KeyA.D, KeyA.D);
    return T;
  }

  public static bool D2UD() {
    Acted(KeyA.L, KeyA.L);
    Acted(KeyA.R, KeyA.R);
    Acted(KeyA.D, KeyA.D);
    return T;
  }

  public static bool D2UA() {
    Acted(KeyA.L, KeyA.L);
    Acted(KeyA.R, KeyA.R);
    Acted(KeyA.D, KeyA.D);
    return T;
  }

  public static bool D2DW() {
    Acted(KeyA.U, KeyA.U);
    Acted(KeyA.D, KeyA.D);
    return T;
  }

  public static bool D2DS() {
    Acted(KeyA.U, KeyA.U);
    Acted(KeyA.D, KeyA.D);
    return T;
  }

  public static bool D2DD() {
    Acted(KeyA.L, KeyA.L);
    Acted(KeyA.R, KeyA.R);
    Act(KeyA.D, KeyX.W);
    return T;
  }

  public static bool D2DA() {
    Acted(KeyA.L, KeyA.L);
    Acted(KeyA.R, KeyA.R);
    Act(KeyA.D, KeyX.W);
    return T;
  }

  public static bool D1UL() {
    State([
      [KeyE.C, KeyE.C],
      [KeyA.D, KeyA.D],
      [KeyA.U, KeyA.U],
      [KeyA.L, KeyA.L],
      [KeyA.R, KeyA.R],
    ], F);
    return T;
  }

  public static bool D1DL() {
    return T switch {
      var _ when IsHeld(KeyX.A) || IsHeld(KeyX.D) || IsHeld(KeyX.S) || IsHeld(KeyX.W) => Perform(),
      _ => Player(),
    };
  }

  public static bool Perform() {
    Strict();
    Player();
    return T;
  }

  public static bool Strict() {
    State([
      [KeyA.D, KeyX.W],
      [KeyA.U, KeyX.S],
      [KeyA.L, KeyX.D],
      [KeyA.R, KeyX.A],
    ], T);

    return T;
  }

  public static bool Player() {
    Fabian(1, 1, KeyE.C, KeyM.L);
    return T;
  }

  public static Task<bool> Fabian(int n2, int n1, uint k2, uint k1) {
    return IsHeld(k1) ? Task.Run(async () => {
      await Keyboard.Hold(n2, k2);
      await Task.Delay(n1);
      return await Fabian(
        n2 + n1,
        n2,
        k2,
        k1
      );
    }) : Task.Run(() => T);
  }

  public static bool State(uint[][] keys, bool type) {
    foreach (uint[] key in keys) {
      _ = type == T ? Act(key[0], key[1]) : Acted(key[0], key[1]);
    }
    return T;
  }

  public static Task<bool> Reactor(int t1, uint k2, uint k1) {
    return IsHeld(k1) ? Task.Run(() => T) : (IsHeld(k2) ? Task.Run(() => T) : Keyboard.Hold(t1, k2));
  }

  public static bool Acted(uint k2, uint k1) {
    return IsHeld(k1) ? (IsHeld(k2) ? Keyboard.Input(k2, F) : T) : T;
  }

  public static bool Act(uint k2, uint k1) {
    return IsHeld(k1) ? (IsHeld(k2) ? T : Keyboard.Input(k2, T)) : T;
  }

  public static bool IsHeld(uint k1) {
    return Held.TryGetValue(k1, out bool is_held) && is_held;
  }

  public static bool OnD2U(uint k1) {
    Held[k1] = F;
    return T switch {
      var _ when KeyX.W == k1 => D2UW(),
      var _ when KeyX.S == k1 => D2US(),
      var _ when KeyX.D == k1 => D2UD(),
      var _ when KeyX.A == k1 => D2UA(),
      _ => T,
    };
  }

  public static bool OnD2D(uint k1) {
    Held[k1] = T;
    return T switch {
      var _ when KeyX.W == k1 => D2DW(),
      var _ when KeyX.S == k1 => D2DS(),
      var _ when KeyX.D == k1 => D2DD(),
      var _ when KeyX.A == k1 => D2DA(),
      _ => T,
    };
  }

  public static bool OnD1U(uint k1) {
    Held[k1] = F;
    return T switch {
      var _ when KeyM.L == k1 => D1UL(),
      _ => T,
    };
  }

  public static bool OnD1D(uint k1) {
    Held[k1] = T;
    return T switch {
      var _ when KeyM.L == k1 => D1DL(),
      _ => T,
    };
  }

  public static readonly Dictionary<uint, bool> Held = [];

  private static IntPtr SetHook(Delegate proc, uint hookType) {
    using ProcessModule? module = Process.GetCurrentProcess().MainModule;

    switch (T) {
      case var _ when module == null:
        return IntPtr.Zero;
      default:
        IntPtr handle = GetModuleHandle(module.ModuleName);

        return T switch {
          var _ when handle == IntPtr.Zero => IntPtr.Zero,
          _ => SetWindowsHookEx((int)hookType, proc, handle, 0),
        };
    }
  }

  private static IntPtr D2HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      uint key = (uint)Marshal.ReadInt32(lParam);
      uint act = (uint)wParam;
      switch (T) {
        case var _ when act == WM_SYSKEYDOWN:
          OnD2D(key);
          return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
        case var _ when act == WM_KEYDOWN:
          OnD2D(key);
          return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
        case var _ when act == WM_SYSKEYUP:
          OnD2U(key);
          return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
        case var _ when act == WM_KEYUP:
          OnD2U(key);
          return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
        default:
          return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
      }
    }
    return CallNextHookEx(d2_hook_id, nCode, wParam, lParam);
  }

  private static IntPtr D1HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
    if (nCode >= 0) {
      uint act = (uint)wParam;
      switch (T) {
        case var _ when act == WM_LBUTTONDOWN:
          OnD1D(KeyM.L);
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
        case var _ when act == WM_LBUTTONUP:
          OnD1U(KeyM.L);
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
        case var _ when act == WM_RBUTTONDOWN:
          OnD1D(0x02);
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
        case var _ when act == WM_RBUTTONUP:
          OnD1U(0x02);
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
        default:
          return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
      }
    }
    return CallNextHookEx(d1_hook_id, nCode, wParam, lParam);
  }

  private static void SubscribeKey(MSG msg) {
    while (GetMessage(out msg, IntPtr.Zero, 0, 0)) {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }
  }

  private static bool Detach(nint id) {
    return T switch {
      var _ when id == IntPtr.Zero => F,
      _ => UnhookWindowsHookEx(id),
    };
  }

  private static void Main() {
    d2_hook_id = SetHook(d2_hook, WH_KEYBOARD_LL);
    d1_hook_id = SetHook(d1_hook, WH_MOUSE_LL);

    SubscribeKey(new MSG());

    Detach(d2_hook_id);
    Detach(d1_hook_id);
  }

  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
  private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

  private static readonly LowLevelKeyboardProc d2_hook = D2HookCallback;
  private static readonly LowLevelMouseProc d1_hook = D1HookCallback;

  private static IntPtr d2_hook_id = IntPtr.Zero;
  private static IntPtr d1_hook_id = IntPtr.Zero;

  private static readonly bool F = false;
  private static readonly bool T = true;

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

  [DllImport("user32.dll")]
  private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

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
}

//public static DateTime since = DateTime.MinValue;
//public static bool is_v_held = F;

//public static bool SubscribeVoice() {
//  var waveIn = new WaveInEvent {
//    WaveFormat = new WaveFormat(44100, 1),
//    BufferMilliseconds = 100
//  };

//  waveIn.DataAvailable += (sender, e) => {
//    float current = HighestVolume(e.Buffer, e.BytesRecorded);
//    double needed = 20 * Math.Log10(current);

//    if (needed > -40 && !is_v_held) {
//      since = DateTime.Now;
//      Keyboard.Emulate(Key.V, T);
//      is_v_held = true;
//    } else if (is_v_held && (DateTime.Now - since).TotalMilliseconds >= 2000) {
//      Keyboard.Emulate(Key.V, F);
//      is_v_held = false;
//    }
//  };

//  waveIn.StartRecording();
//  return T;
//}

//public static float HighestVolume(byte[] buffer, int bytesRecorded) {
//  float maxVolume = 0;
//  for (int i = 0; i < bytesRecorded; i += 2) {
//    short sample = BitConverter.ToInt16(buffer, i);
//    float sample32 = sample / 32768f;

//    if (sample32 < 0) { sample32 = -sample32; }
//    if (sample32 > maxVolume) { maxVolume = sample32; }
//  }
//  return maxVolume;
//}

//HoldF(KeyA.D, KeyA.D);
//HoldF(KeyA.U, KeyA.U);
//HoldF(KeyA.R, KeyA.R);
//HoldF(KeyA.L, KeyA.L);

//HoldT(KeyA.D, Key.W);
//HoldT(KeyA.U, Key.S);
//HoldT(KeyA.R, Key.A);
//HoldT(KeyA.L, Key.D);
