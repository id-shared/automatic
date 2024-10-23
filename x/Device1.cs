﻿using System.Runtime.InteropServices;

class Device1 {
  public Device1(string c) {
    context = new(@$"\??\ROOT#SYSTEM#0001#{{{c}}}");
  }

  public bool YX(int y, int x) {
    return Act(new MouseReport { y = (short)y , x = (short)x }, 0x2A2010, A.T);
  }

  public bool E(int e) {
    // HINT: Act(new MouseReport { Button = new MouseButton { LButton = A.T }, y = 0, x = 0 }, 0x2A2010, A.T);

    return e switch {
      2 => Device2.Input([KeyM.L], A.F),
      1 => Device2.Input([KeyM.L], A.T),
      _ => Device2.Input([KeyM.L], A.T),
    };
  }

  public static bool Input(uint[] k, bool a) {
    Native.INPUT[] inputs = new Native.INPUT[k.Length];
    for (int i = 0; i < k.Length; i++) {
      inputs[i].type = 0;
      inputs[i].mkhi.ki.wVk = (ushort)k[i];
      inputs[i].mkhi.ki.wScan = 0;
      inputs[i].mkhi.ki.dwFlags = a ? E_KEYD : E_KEYU;
      inputs[i].mkhi.ki.time = 0;
      inputs[i].mkhi.ki.dwExtraInfo = IntPtr.Zero;
    }
    return Native.SendInput((uint)inputs.Length, inputs, Native.INPUT_SIZE) != 0;
  }

  public bool Act<X>(X x, uint e, bool a) {
    IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(x));

    try {
      Marshal.StructureToPtr(x, buffer, false);
      uint bytesReturned = 0;

      return a switch {
        A.T => Native.DeviceIoControl(context.contact, e, buffer, (uint)Marshal.SizeOf(x), IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero),
        _ => Native.DeviceIoControl(context.contact, e, IntPtr.Zero, 0, buffer, (uint)Marshal.SizeOf(x), out bytesReturned, IntPtr.Zero),
      };
    } catch {
      return A.F;
    } finally {
      Marshal.FreeHGlobal(buffer);
    }
  }

  public static readonly uint E_KEYU = 0x0002;
  public static readonly uint E_KEYD = 0x0000;
  private readonly Context context;
}

[StructLayout(LayoutKind.Explicit, Size = 8)]
struct MouseReport {
  public MouseButton Button {
    get => button;
    set => button = value;
  }

  public byte ButtonByte {
    get => button_byte;
    set => button_byte = value;
  }

  [FieldOffset(0)]
  private MouseButton button;

  [FieldOffset(0)]
  private byte button_byte;

  [FieldOffset(2)]
  public short x;

  [FieldOffset(4)]
  public short y;

  [FieldOffset(6)]
  public byte wheel;

  [FieldOffset(7)]
  public byte unknown_T;
}

struct MouseButton {
  private byte buttons;

  public bool LButton {
    get => (buttons & 0b00000001) != 0;
    set {
      if (value)
        buttons |= 0b00000001;
      else
        buttons &= 0b11111110;
    }
  }

  public bool RButton {
    get => (buttons & 0b00000010) != 0;
    set {
      if (value)
        buttons |= 0b00000010;
      else
        buttons &= 0b11111101;
    }
  }

  public bool MButton {
    get => (buttons & 0b00000100) != 0;
    set {
      if (value)
        buttons |= 0b00000100;
      else
        buttons &= 0b11111011;
    }
  }

  public bool XButton1 {
    get => (buttons & 0b00001000) != 0;
    set {
      if (value)
        buttons |= 0b00001000;
      else
        buttons &= 0b11110111;
    }
  }

  public bool XButton2 {
    get => (buttons & 0b00010000) != 0;
    set {
      if (value)
        buttons |= 0b00010000;
      else
        buttons &= 0b11101111;
    }
  }

  public bool Unknown {
    get => (buttons & 0b11100000) != 0;
    set {
      if (value)
        buttons |= 0b11100000;
      else
        buttons &= 0b00011111;
    }
  }
}
