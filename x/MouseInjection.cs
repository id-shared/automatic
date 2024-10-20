using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

public static class MouseInjection {
  // Call this method to initialize the driver and start capturing input
  public static void Initialize() {
    SafeFileHandle fs = MouClassInput.InitializeDeviceHandle();

    Console.WriteLine("Abc.");
    // Prepare a structure to hold the mouse device stack information
    MOUSE_DEVICE_STACK_INFORMATION deviceStackInfo = new();

    bool abc = MouiiIoInitializeMouseDeviceStackContext(fs, ref deviceStackInfo);

    //Console.WriteLine(IoctlCodes.IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT);
    Console.WriteLine(deviceStackInfo.ButtonDevice.UnitId);
    Console.WriteLine(deviceStackInfo.MovementDevice.UnitId);
    Console.WriteLine(deviceStackInfo);

    Thread.Sleep(2000);

    bool abc2 = MouiiIoInjectMouseMovementInput(fs, 4012, 0, 9, 9);

    Console.WriteLine("yy");
    Console.WriteLine(abc2);
    Console.WriteLine("xx");

    //uint cbReturned = 0;

    //IntPtr inBuffer = IntPtr.Zero;
    //IntPtr outBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(deviceStackInformation));

    //if (!DeviceIoControl(
    //  _deviceHandle,
    //  IoctlCodes.IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT,
    //  inBuffer,
    //  0,
    //  outBuffer,
    //  (uint)Marshal.SizeOf<MOUSE_DEVICE_STACK_INFORMATION>(),
    //  ref cbReturned,
    //  IntPtr.Zero)
    //) {
    //  throw new InvalidOperationException("Failed to initialize driver. Error code: " + Marshal.GetLastWin32Error());
    //}

    Console.WriteLine("Driver initialized and listening for input.");
  }

  public static bool MouiiIoInitializeMouseDeviceStackContext(SafeFileHandle handle, ref MOUSE_DEVICE_STACK_INFORMATION pDeviceStackInformation) {
    // Prepare for the IOCTL request
    uint bytesReturned = 0;
    // This assumes the device stack information is zeroed out before this call
    IntPtr pStackInfoPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(MOUSE_DEVICE_STACK_INFORMATION)));

    try {
      // Call the DeviceIoControl function
      bool result = DeviceIoControl(
          handle,
          IoctlCodes.IOCTL_INITIALIZE_MOUSE_DEVICE_STACK_CONTEXT,
          IntPtr.Zero,       // No input buffer needed
          0,                 // Input buffer size
          pStackInfoPtr,    // Output buffer
          (uint)Marshal.SizeOf(typeof(MOUSE_DEVICE_STACK_INFORMATION)), // Output buffer size
          out bytesReturned, // Bytes returned
          IntPtr.Zero);      // No overlapped structure needed

      if (!result) {
        // Handle the error as needed
        return false;
      }

      // Marshal the data from the pointer to the structure
      pDeviceStackInformation = Marshal.PtrToStructure<MOUSE_DEVICE_STACK_INFORMATION>(pStackInfoPtr);
      return true;
    } finally {
      // Free the allocated memory
      Marshal.FreeHGlobal(pStackInfoPtr);
    }
  }

  public static bool MouiiIoInjectMouseMovementInput(SafeFileHandle handle, nint processId, ushort indicatorFlags, int movementX, int movementY) {
    INJECT_MOUSE_MOVEMENT_INPUT_REQUEST request = new() {
      ProcessId = processId,
      IndicatorFlags = indicatorFlags,
      MovementX = movementX,
      MovementY = movementY
    };

    uint bytesReturned = 0;

    IntPtr requestPtr = Marshal.AllocHGlobal(Marshal.SizeOf(request));

    try {
      Marshal.StructureToPtr(request, requestPtr, false);

      Console.WriteLine(IoctlCodes.IOCTL_INJECT_MOUSE_MOVEMENT_INPUT);

      bool status = DeviceIoControl(
        handle,
        IoctlCodes.IOCTL_INJECT_MOUSE_MOVEMENT_INPUT,
        requestPtr,
        (uint)Marshal.SizeOf(request),
        IntPtr.Zero,
        0,
        out bytesReturned,
        IntPtr.Zero
      );

      if (!status) {
        throw new InvalidOperationException("DeviceIoControl x. Error: " + Marshal.GetLastWin32Error());
      }

      if (bytesReturned == 0) {
        throw new InvalidOperationException("DeviceIoControl x succeeded, but no data was written.");
      }

      return status;
    } finally {
      Marshal.FreeHGlobal(requestPtr);
    }
  }

  [DllImport("kernel32.dll", SetLastError = true)]
  public static extern bool DeviceIoControl(
      SafeFileHandle hDevice,
      uint dwIoControlCode,
      IntPtr lpInBuffer,
      uint nInBufferSize,
      IntPtr lpOutBuffer,
      uint nOutBufferSize,
      out uint lpBytesReturned,
      IntPtr lpOverlapped);
}

public class MouClassInput {
  // Importing CreateFile from kernel32.dll
  [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
  private static extern SafeFileHandle CreateFile(
      string lpFileName,
      uint dwDesiredAccess,
      uint dwShareMode,
      IntPtr lpSecurityAttributes,
      uint dwCreationDisposition,
      uint dwFlagsAndAttributes,
      IntPtr hTemplateFile
  );

  // Constants for CreateFile
  private const uint GENERIC_READ = 0x80000000;
  private const uint GENERIC_WRITE = 0x40000000;
  private const uint FILE_SHARE_READ = 0x00000001;
  private const uint FILE_SHARE_WRITE = 0x00000002;
  private const uint OPEN_EXISTING = 3;
  private const uint FILE_ATTRIBUTE_NORMAL = 0x80;

  // The device path must match the symbolic link to your MouClassInput driver.
  private const string DevicePath = @"\\.\Device1";

  public static SafeFileHandle InitializeDeviceHandle() {
    // Acquire the device handle
    return CreateFile(
        DevicePath,
        GENERIC_READ | GENERIC_WRITE,
        FILE_SHARE_READ | FILE_SHARE_WRITE,
        IntPtr.Zero,
        OPEN_EXISTING,
        FILE_ATTRIBUTE_NORMAL,
        IntPtr.Zero
    );
  }

  // Importing CloseHandle from kernel32.dll
  [DllImport("kernel32.dll", SetLastError = true)]
  private static extern bool CloseHandle(SafeFileHandle hObject);
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

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct INJECT_MOUSE_MOVEMENT_INPUT_REQUEST {
  public nint ProcessId;
  public ushort IndicatorFlags;
  public int MovementX;
  public int MovementY;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct INJECT_MOUSE_BUTTON_INPUT_REQUEST {
  public nint ProcessId; // Corrected to UIntPtr.
  public ushort ButtonFlags;
  public ushort ButtonData;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct INJECT_MOUSE_INPUT_PACKET_REQUEST {
  public UIntPtr ProcessId; // Corrected to UIntPtr.
  public bool UseButtonDevice;
  public MOUSE_INPUT_DATA InputPacket;
}

// Assuming MOUSE_INPUT_DATA is defined elsewhere
[StructLayout(LayoutKind.Sequential)]
public struct MOUSE_INPUT_DATA {
  // Define members here based on your requirements
}
