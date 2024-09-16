﻿using System.Runtime.InteropServices;

class Keyboard {
  public static readonly bool F = false;
  public static readonly bool T = true;

  public static bool IsHeld(uint key) {
    return (GetKeyState(key) & 0x8000) != 0;
  }

  public static bool Hold(uint key, int time) {
    I(key, T);
    _ = new System.Threading.Timer(_ => Keyboard.I(key, F), null, time, Timeout.Infinite);
    return T;
  }

  public static uint I(uint key, bool is_pressed) {
    INPUT[] inputs = new INPUT[1];

    inputs[0].type = INPUT_KEYBOARD;
    inputs[0].mkhi.ki = new KEYBDINPUT {
      wVk = (ushort)key,
      wScan = 0,
      dwFlags = is_pressed ? 0 : KEYEVENTF_KEYUP,
      time = 0,
      dwExtraInfo = IntPtr.Zero
    };

    return SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
  }

  private const uint INPUT_KEYBOARD = 1;
  private const uint KEYEVENTF_KEYUP = 0x0002;

  [StructLayout(LayoutKind.Sequential)]
  private struct INPUT {
    public uint type;
    public MOUSEKEYBDHARDWAREINPUT mkhi;
  }

  [StructLayout(LayoutKind.Explicit)]
  private struct MOUSEKEYBDHARDWAREINPUT {
    [FieldOffset(0)]
    public MOUSEINPUT mi;
    [FieldOffset(0)]
    public KEYBDINPUT ki;
    [FieldOffset(0)]
    public HARDWAREINPUT hi;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct KEYBDINPUT {
    public ushort wVk;
    public ushort wScan;
    public uint dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct MOUSEINPUT {
    public int dx;
    public int dy;
    public uint mouseData;
    public uint dwFlags;
    public uint time;
    public IntPtr dwExtraInfo;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct HARDWAREINPUT {
    public uint uMsg;
    public ushort wParamL;
    public ushort wParamH;
  }

  [DllImport("user32.dll")]
  private static extern short GetKeyState(uint vKey);

  [DllImport("user32.dll")]
  private static extern uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);

  [DllImport("user32.dll")]
  private static extern uint MapVirtualKey(uint uCode, uint uMapType);
}
