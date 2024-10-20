using System.Runtime.InteropServices;

class Device3 {
  public Device3(IntPtr e, string c) {
    context = new(@$"\\.\{c}");
    process = e;
    _ = Act(new MOUSE_DEVICE_STACK_INFORMATION(), code.IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT, A.F) ? A.T : throw new InvalidOperationException(nameof(Device3));
  }

  public bool YX(int y, int x) {
    return Act(new InjectMouseMovementInputRequest() {
      ProcessId = process,
      IndicatorFlags = 0,
      MovementX = x,
      MovementY = y,
    }, code.IOCTL_INJECT_MOUSE_MOVEMENT_INPUT, A.T);
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
    IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT = CTL_CODE(AX, 2600, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);

    IOCTL_INJECT_MOUSE_MOVEMENT_INPUT = CTL_CODE(AX, 2851, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);

    IOCTL_INJECT_MOUSE_BUTTON_INPUT = CTL_CODE(AX, 2850, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);

    IOCTL_INJECT_MOUSE_INPUT_PACKET = CTL_CODE(AX, 2870, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);
  }

  private static uint CTL_CODE(uint deviceType, uint function, DeviceMethod method, FileAccess access) {
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

  public readonly uint IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT;
  public readonly uint IOCTL_INJECT_MOUSE_MOVEMENT_INPUT;
  public readonly uint IOCTL_INJECT_MOUSE_BUTTON_INPUT;
  public readonly uint IOCTL_INJECT_MOUSE_INPUT_PACKET;
  public const uint AX = 48781u;
}

[StructLayout(LayoutKind.Sequential)]
public struct MOUSE_CLASS_BUTTON_DEVICE_INFORMATION {
  public ushort UnitId;
}

[StructLayout(LayoutKind.Sequential)]
public struct MOUSE_CLASS_MOVEMENT_DEVICE_INFORMATION {
  public ushort UnitId;
  [MarshalAs(UnmanagedType.I1)]
  public bool AbsoluteMovement;
  [MarshalAs(UnmanagedType.I1)]
  public bool VirtualDesktop;
}

[StructLayout(LayoutKind.Sequential)]
public struct MOUSE_DEVICE_STACK_INFORMATION {
  public MOUSE_CLASS_BUTTON_DEVICE_INFORMATION ButtonDevice;
  public MOUSE_CLASS_MOVEMENT_DEVICE_INFORMATION MovementDevice;
}

[StructLayout(LayoutKind.Sequential)]
public struct InjectMouseMovementInputRequest {
  public IntPtr ProcessId;
  public ushort IndicatorFlags;
  public int MovementX;
  public int MovementY;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct INJECT_MOUSE_INPUT_PACKET_REQUEST {
  public UIntPtr ProcessId;
  public bool UseButtonDevice;
  public MOUSE_INPUT_DATA InputPacket;
}

// Assuming MOUSE_INPUT_DATA is defined elsewhere
[StructLayout(LayoutKind.Sequential)]
public struct MOUSE_INPUT_DATA {
  // Define members here based on your requirements
}
