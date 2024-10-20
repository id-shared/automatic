using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

class Device3 {
  public Device3() {
    context = new(@"\\.\Device1");
    MOUSE_DEVICE_STACK_INFORMATION i = Initialize();
    Console.WriteLine($"Next: {i.ButtonDevice.UnitId}");
    bool a = Move();
    Console.WriteLine($"Move: {a}");
  }

  public MOUSE_DEVICE_STACK_INFORMATION Initialize() {
    MOUSE_DEVICE_STACK_INFORMATION deviceStackInfo = new();
    uint cbReturned = 0;

    IntPtr deviceInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(deviceStackInfo));

    try {
      IntPtr outBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(deviceStackInfo));
      Marshal.StructureToPtr(deviceStackInfo, outBuffer, false);

      bool status = Native.DeviceIoControl(
        context.contact,
        code.IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT,
        IntPtr.Zero,
        0,
        outBuffer,
        (uint)Marshal.SizeOf(deviceStackInfo),
        out cbReturned,
        IntPtr.Zero
      );

      if (!status) {
        Console.WriteLine($"DeviceIoControl failed: {Marshal.GetLastWin32Error()}");
      }

      Console.WriteLine(cbReturned);
    } finally {
      Marshal.FreeHGlobal(deviceInfoPtr);
    }

    return deviceStackInfo;
  }

  public bool Move() {
    return React(new InjectMouseMovementInputRequest() {
      ProcessId = 4012,
      IndicatorFlags = 0,
      MovementX = 10,
      MovementY = 10
    }, code.IOCTL_INJECT_MOUSE_MOVEMENT_INPUT, A.F);
  }

  public bool React<X>(X x, uint e, bool a) {
    IntPtr buffer = Marshal.AllocHGlobal(Marshal.SizeOf(x));

    try {
      Marshal.StructureToPtr(x, buffer, false);
      uint bytesReturned = 0;

      return Native.DeviceIoControl(
        context.contact,
        e,
        buffer,
        (uint)Marshal.SizeOf(x),
        IntPtr.Zero,
        0,
        out bytesReturned,
        IntPtr.Zero
      );
    } catch {
      return A.F;
    } finally {
      Marshal.FreeHGlobal(buffer);
    }
  }

  //public bool DeviceIoControl(bool a) => A.T switch {
  //  a => Native.DeviceIoControl(
  //      context.contact,
  //      e,
  //      outBuffer,
  //      (uint)Marshal.SizeOf(x),
  //      IntPtr.Zero,
  //      0,
  //      out bytesReturned,
  //      IntPtr.Zero
  //    ),
  //  _ => Native.DeviceIoControl(
  //      context.contact,
  //      e,
  //      outBuffer,
  //      (uint)Marshal.SizeOf(x),
  //      IntPtr.Zero,
  //      0,
  //      out bytesReturned,
  //      IntPtr.Zero
  //    ),
  //};

  private readonly IOCode code = new();
  private readonly Context context;
}

partial class Context : IDisposable {
  public SafeFileHandle contact = new(IntPtr.Zero, true);
  public bool isDisconnected = false;

  public Context(string e) {
    contact = Native.CreateFile(
      e,
      GENERIC_READ | GENERIC_WRITE,
      FILE_SHARE_READ | FILE_SHARE_WRITE,
      IntPtr.Zero,
      OPEN_EXISTING,
      FILE_ATTRIBUTE_NORMAL,
      IntPtr.Zero
    );

    if (contact.IsInvalid) {
      contact.Dispose();
    }
  }

  public void Terminate() {
    if (!contact.IsInvalid) {
      contact.Dispose();
    }
  }

  public void Dispose() {
    if (!isDisconnected) {
      Terminate();
      isDisconnected = true;
    }

    GC.SuppressFinalize(this);
  }

  private const uint GENERIC_READ = 0x80000000;
  private const uint GENERIC_WRITE = 0x40000000;
  private const uint FILE_SHARE_READ = 0x00000001;
  private const uint FILE_SHARE_WRITE = 0x00000002;
  private const uint OPEN_EXISTING = 3;
  private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
}

class Native {
  [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  public static extern bool DeviceIoControl(
    SafeFileHandle hDevice,
    uint dwIoControlCode,
    IntPtr lpInBuffer,
    uint nInBufferSize,
    IntPtr lpOutBuffer,
    uint nOutBufferSize,
    out uint lpBytesReturned,
    IntPtr lpOverlapped
  );

  [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
  public static extern SafeFileHandle CreateFileW(
    string lpFileName,
    uint dwDesiredAccess,
    uint dwShareMode,
    IntPtr lpSecurityAttributes,
    uint dwCreationDisposition,
    uint dwFlagsAndAttributes,
    IntPtr hTemplateFile
  );

  [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
  public static extern SafeFileHandle CreateFileA(
    string lpFileName,
    uint dwDesiredAccess,
    uint dwShareMode,
    IntPtr lpSecurityAttributes,
    uint dwCreationDisposition,
    uint dwFlagsAndAttributes,
    IntPtr hTemplateFile
  );

  [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
  public static extern SafeFileHandle CreateFile(
    string lpFileName,
    uint dwDesiredAccess,
    uint dwShareMode,
    IntPtr lpSecurityAttributes,
    uint dwCreationDisposition,
    uint dwFlagsAndAttributes,
    IntPtr hTemplateFile);

  [DllImport("kernel32.dll", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool CloseHandle(IntPtr hObject);
}

class IOCode {
  public IOCode() {
    IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT = CTL_CODE(FILE_DEVICE_MOUCLASS_INPUT_INJECTION, 2600, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);

    IOCTL_INJECT_MOUSE_MOVEMENT_INPUT = CTL_CODE(FILE_DEVICE_MOUCLASS_INPUT_INJECTION, 2851, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);

    IOCTL_INJECT_MOUSE_BUTTON_INPUT = CTL_CODE(FILE_DEVICE_MOUCLASS_INPUT_INJECTION, 2850, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);

    IOCTL_INJECT_MOUSE_INPUT_PACKET = CTL_CODE(FILE_DEVICE_MOUCLASS_INPUT_INJECTION, 2870, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);
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
  public const uint FILE_DEVICE_MOUCLASS_INPUT_INJECTION = 48781u;
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
