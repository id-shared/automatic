using System.Runtime.InteropServices;

class Device11 {
  public Device11(IntPtr e, string c) {
    context = new(@$"\\.\{c}");
    process = e;
    _ = Act(new ContextO(), code.CONTEXT, A.F) ? A.T : throw new InvalidOperationException(nameof(Device11));
  }

  public bool YX(int y, int x) {
    return Act(new MoveI() {
      ProcessId = process,
      IndicatorFlags = 0,
      MovementX = x,
      MovementY = y,
    }, code.MOVE, A.T);
  }

  public bool E(ushort e) {
    return Act(new BtnI() {
      ProcessId = process,
      ButtonFlags = e,
      ButtonData = 0
    }, code.BTN, A.T);
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

  private readonly IOCode code = new();
  private readonly IntPtr process;
  private readonly Context context;
}

class IOCode {
  public IOCode() {
    CONTEXT = Code(CODE, 2600, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);
    PACKET = Code(CODE, 2870, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);
    MOVE = Code(CODE, 2851, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);
    BTN = Code(CODE, 2850, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);
  }

  private static uint Code(uint deviceType, uint function, DeviceMethod method, FileAccess access) {
    return ((deviceType << 16) | ((uint)access << 14) | (function << 2) | (uint)method);
  }

  public enum FileAccess {
    FileAnyAccess = 0,
    FileReadAccess = 1,
    FileWriteAccess = 2
  }

  public enum DeviceMethod {
    MethodBuffered = 0,
    MethodInDirect = 1,
    MethodOutDirect = 2,
    MethodNeither = 3
  }

  public readonly uint CONTEXT;
  public readonly uint BTN;
  public readonly uint PACKET;
  public readonly uint MOVE;
  public readonly uint CODE = 48781u;
}

[StructLayout(LayoutKind.Sequential)]
public struct MoveI {
  public IntPtr ProcessId;
  public ushort IndicatorFlags;
  public int MovementX;
  public int MovementY;
}

[StructLayout(LayoutKind.Sequential)]
public struct BtnI {
  public IntPtr ProcessId;
  public ushort ButtonFlags;
  public ushort ButtonData;
}

[StructLayout(LayoutKind.Sequential)]
public struct ContextO {
  public ContextButtonO ButtonDevice;
  public ContextMoveO MovementDevice;
}

[StructLayout(LayoutKind.Sequential)]
public struct ContextButtonO {
  public ushort UnitId;
}

[StructLayout(LayoutKind.Sequential)]
public struct ContextMoveO {
  [MarshalAs(UnmanagedType.I1)]
  public bool AbsoluteMovement;
  [MarshalAs(UnmanagedType.I1)]
  public bool VirtualDesktop;
  public ushort UnitId;
}
