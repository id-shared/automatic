using System.Runtime.InteropServices;

class Xyloid2 {
  public bool YX(int y, int x) {
    D1Control control = d1control;
    control.mi.LastY = y * -1;
    control.mi.LastX = x;

    return Act(control, A.T);
  }

  public bool E1(bool a) {
    return A.T;
  }

  public bool Act<X>(X x, bool a) {
    IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(x));

    try {
      Marshal.StructureToPtr(x, buffer, false);
      uint bytesReturned = 0;

      return a switch {
        A.T => Native.DeviceIoControl(context.contact, CODE, buffer, (uint)Marshal.SizeOf(x), IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero),
        _ => Native.DeviceIoControl(context.contact, CODE, IntPtr.Zero, 0, buffer, (uint)Marshal.SizeOf(x), out bytesReturned, IntPtr.Zero),
      };
    } catch {
      return A.F;
    } finally {
      Marshal.FreeHGlobal(buffer);
    }
  }

  public Xyloid2(string c) {
    context = new(c);
  }

  private readonly D1Control d1control = new D1Control {
    type = D1ControlType.Mouse,
    mi = new MOUSE_INPUT_DATA()
  };
  private readonly uint CODE = 0x88883020;
  private readonly Context context;
}

[StructLayout(LayoutKind.Sequential)]
public struct KEYBOARD_INPUT_DATA {
  public ushort UnitId;
  public ushort MakeCode;
  public ushort Flags;
  public ushort Reserved;
  public uint ExtraInformation;
}

[StructLayout(LayoutKind.Explicit)]
public struct MOUSE_INPUT_DATA {
  [FieldOffset(0)]
  public ushort UnitId;

  [FieldOffset(2)]
  public ushort Flags;

  // Union: Buttons can be accessed as a single uint or as ButtonFlags and ButtonData
  [FieldOffset(4)]
  public uint Buttons;

  [FieldOffset(4)]
  public ushort ButtonFlags;

  [FieldOffset(6)]
  public ushort ButtonData;

  [FieldOffset(8)]
  public uint RawButtons;

  [FieldOffset(12)]
  public int LastX;

  [FieldOffset(16)]
  public int LastY;

  [FieldOffset(20)]
  public uint ExtraInformation;
}

public enum D1ControlType : uint {
  Keyboard = 1,
  Mouse = 2
}

[StructLayout(LayoutKind.Explicit, Size = 32)]
public struct D1Control {
  [FieldOffset(0)]
  public uint unk1;

  [FieldOffset(4)]
  public D1ControlType type;

  [FieldOffset(8)]
  public KEYBOARD_INPUT_DATA ki;

  [FieldOffset(8)]
  public MOUSE_INPUT_DATA mi;
}
