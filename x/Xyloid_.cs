using System.Runtime.InteropServices;

public class Xyloid {
  public bool Act(Xyloid_ x, bool a) {
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

  public Xyloid(string c) {
    context = new(c);
  }

  private readonly uint CODE = 0x88883020;
  private readonly Context context;
}

[StructLayout(LayoutKind.Explicit, Size = 32)]
public struct Xyloid_ {
  [FieldOffset(0)]
  public uint unk1;

  [FieldOffset(4)]
  public XyloidType type;

  [FieldOffset(8)]
  public KEYBOARD_INPUT_DATA ki;

  [FieldOffset(8)]
  public MOUSE_INPUT_DATA mi;
}

public enum XyloidType : uint {
  Keyboard = 1,
  Mouse = 2
}

[StructLayout(LayoutKind.Explicit)]
public struct KEYBOARD_INPUT_DATA {
  [FieldOffset(0)]
  public ushort UnitId;

  [FieldOffset(2)]
  public ushort MakeCode;

  [FieldOffset(4)]
  public ushort Flags;

  [FieldOffset(6)]
  public ushort Reserved;

  [FieldOffset(8)]
  public uint ExtraInformation;
}

[StructLayout(LayoutKind.Explicit)]
public struct MOUSE_INPUT_DATA {
  [FieldOffset(0)]
  public ushort UnitId;

  [FieldOffset(2)]
  public ushort Flags;

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
