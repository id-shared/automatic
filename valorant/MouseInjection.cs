using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

public static class MouseInjection {
  private static readonly DriverHandleManager _handleManager = new();
  private static bool _isInitialized = false;
  private static SafeFileHandle _deviceHandle = null;

  // Call this method to initialize the driver and start capturing input
  public static void Initialize() {
    if (!_isInitialized) {
      _deviceHandle = _handleManager.GetHandle();

      if (_deviceHandle.IsInvalid) {
        throw new InvalidOperationException("Failed to obtain a valid handle to the device. Check if the driver is installed and the device path is correct.");
      }

      Console.WriteLine(IoctlCodes.IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT);

      MOUSE_DEVICE_STACK_INFORMATION deviceStackInformation = new();
      uint cbReturned = 0;

      IntPtr inBuffer = IntPtr.Zero;
      IntPtr outBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(deviceStackInformation));

      if (!DeviceIoControl(
        _deviceHandle,
        IoctlCodes.IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT,
        inBuffer,
        0,
        outBuffer,
        (uint)Marshal.SizeOf<MOUSE_DEVICE_STACK_INFORMATION>(),
        ref cbReturned,
        IntPtr.Zero)
      ) {
        throw new InvalidOperationException("Failed to initialize driver. Error code: " + Marshal.GetLastWin32Error());
      }

      _isInitialized = true;
      Console.WriteLine("Driver initialized and listening for input.");
    }
  }

  [DllImport("kernel32.dll", SetLastError = true)]
  private static extern bool DeviceIoControl(
    SafeFileHandle hDevice,
    uint dwIoControlCode,
    IntPtr lpInBuffer,
    uint nInBufferSize,
    IntPtr lpOutBuffer,
    uint nOutBufferSize,
    ref uint lpBytesReturned,
    IntPtr lpOverlapped
  );
}

public class DriverHandleManager : IDisposable {
  private const uint OPEN_EXISTING = 3;

  private SafeFileHandle _handle;

  public DriverHandleManager() {
    OpenHandle();
  }

  private void OpenHandle() {
    _handle = CreateFile(@"\\.\Device1", 0xC0000000, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
    if (_handle.IsInvalid) {
      throw new InvalidOperationException("Failed to open device.");
    }
  }

  [DllImport("kernel32.dll", SetLastError = true)]
  private static extern SafeFileHandle CreateFile(
    string lpFileName,
    uint dwDesiredAccess,
    uint dwShareMode,
    IntPtr lpSecurityAttributes,
    uint dwCreationDisposition,
    uint dwFlagsAndAttributes,
    IntPtr hTemplateFile
  );

  public SafeFileHandle GetHandle() {
    if (_handle.IsClosed || _handle.IsInvalid) {
      OpenHandle();
    }
    return _handle;
  }

  public void Dispose() {
    if (_handle != null && !_handle.IsClosed) {
      _handle.Close();
    }
  }
}

//=============================================================================
// Names
//=============================================================================
public static class DeviceNames {
  public const string DRIVER_NAME_U = "Device1";
  public const string LOCAL_DEVICE_PATH_U = @"\\.\Device1";
  public const string NT_DEVICE_NAME_U = @"\Device\Device1";
  public const string SYMBOLIC_LINK_NAME_U = @"\DosDevices\Device1";
}

//=============================================================================
// Ioctls
//=============================================================================
public static class IoctlCodes {
  public const uint FILE_DEVICE_MOUCLASS_INPUT_INJECTION = 48781u;

  public static readonly uint IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT;
  public static readonly uint IOCTL_INJECT_MOUSE_MOVEMENT_INPUT;
  public static readonly uint IOCTL_INJECT_MOUSE_BUTTON_INPUT;
  public static readonly uint IOCTL_INJECT_MOUSE_INPUT_PACKET;

  // Static constructor to initialize the IOCTL codes
  static IoctlCodes() {
    IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT = CTL_CODE(FILE_DEVICE_MOUCLASS_INPUT_INJECTION, 2600, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);

    IOCTL_INJECT_MOUSE_MOVEMENT_INPUT = CTL_CODE(FILE_DEVICE_MOUCLASS_INPUT_INJECTION, 2851, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);

    IOCTL_INJECT_MOUSE_BUTTON_INPUT = CTL_CODE(FILE_DEVICE_MOUCLASS_INPUT_INJECTION, 2850, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);

    IOCTL_INJECT_MOUSE_INPUT_PACKET = CTL_CODE(FILE_DEVICE_MOUCLASS_INPUT_INJECTION, 2870, DeviceMethod.MethodBuffered, FileAccess.FileAnyAccess);
  }

  private static uint CTL_CODE(uint deviceType, uint function, DeviceMethod method, FileAccess access) {
    return ((deviceType << 16) | ((uint)access << 14) | (function << 2) | (uint)method);
  }
}

public enum DeviceMethod {
  MethodBuffered = 0,
  MethodInDirect = 1,
  MethodOutDirect = 2,
  MethodNeither = 3
}

public enum FileAccess : uint {
  FileAnyAccess = 0,
  FileReadAccess = 1,
  FileWriteAccess = 2
}

//=============================================================================
// IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT
//=============================================================================
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
public struct INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT_REPLY {
  public MOUSE_DEVICE_STACK_INFORMATION DeviceStackInformation;
}

//=============================================================================
// IOCTL_INJECT_MOUSE_BUTTON_INPUT
//=============================================================================
[StructLayout(LayoutKind.Sequential)]
public struct INJECT_MOUSE_BUTTON_INPUT_REQUEST {
  public IntPtr ProcessId;  // Use IntPtr for ULONG_PTR
  public ushort ButtonFlags;
  public ushort ButtonData;
}

//=============================================================================
// IOCTL_INJECT_MOUSE_MOVEMENT_INPUT
//=============================================================================
[StructLayout(LayoutKind.Sequential)]
public struct INJECT_MOUSE_MOVEMENT_INPUT_REQUEST {
  public IntPtr ProcessId;  // Use IntPtr for ULONG_PTR
  public ushort IndicatorFlags;
  public int MovementX;  // Use int for LONG
  public int MovementY;  // Use int for LONG
}

//=============================================================================
// IOCTL_INJECT_MOUSE_INPUT_PACKET
//=============================================================================
[StructLayout(LayoutKind.Sequential)]
public struct INJECT_MOUSE_INPUT_PACKET_REQUEST {
  public IntPtr ProcessId;  // Use IntPtr for ULONG_PTR
  [MarshalAs(UnmanagedType.I1)]
  public bool UseButtonDevice;
  public MOUSE_INPUT_DATA InputPacket;  // Assuming MOUSE_INPUT_DATA is defined elsewhere
}

// Assuming MOUSE_INPUT_DATA is defined elsewhere
[StructLayout(LayoutKind.Sequential)]
public struct MOUSE_INPUT_DATA {
  // Define members here based on your requirements
}
