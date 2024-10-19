using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

public static class MouseInjection {
  private static readonly DriverHandleManager _handleManager = new();
  private static bool _isInitialized = false;
  private static SafeFileHandle _deviceHandle = null;

  private const uint IOCTL_INIT_DRIVER = 0x80000000; // Replace with actual IOCTL code from your driver
  private const uint IOCTL_START_LISTENING = 0x80000001; // Replace with actual IOCTL code from your driver
  private const uint IOCTL_INJECT_MOUSE_BUTTON = 0x80000002; // Replace with actual IOCTL code from your driver
  private const uint IOCTL_INJECT_MOUSE_MOVEMENT = 0x80000003; // Replace with actual IOCTL code from your driver

  private const uint METHOD_BUFFERED = 0;
  private const uint FILE_DEVICE_MOUSE = 0x0000000F;
  private const uint FILE_ANY_ACCESS = 0;

  // Call this method to initialize the driver and start capturing input
  public static void Initialize() {
    if (!_isInitialized) {
      _deviceHandle = CreateDeviceHandle();

      if (_deviceHandle.IsInvalid) {
        throw new InvalidOperationException("Failed to obtain a valid handle to the device. Check if the driver is installed and the device path is correct.");
      }

      // Initialize the driver
      if (!DeviceIoControl(_deviceHandle, IOCTL_INIT_DRIVER, IntPtr.Zero, 0, IntPtr.Zero, 0, out _, IntPtr.Zero)) {
        throw new InvalidOperationException("Failed to initialize driver. Error code: " + Marshal.GetLastWin32Error());
      }

      // Start listening for input events
      if (!DeviceIoControl(_deviceHandle, IOCTL_START_LISTENING, IntPtr.Zero, 0, IntPtr.Zero, 0, out _, IntPtr.Zero)) {
        throw new InvalidOperationException("Failed to start listening for input. Error code: " + Marshal.GetLastWin32Error());
      }

      _isInitialized = true;
      Console.WriteLine("Driver initialized and listening for input.");
    }
  }

  private static SafeFileHandle CreateDeviceHandle() {
    SafeFileHandle handle = CreateFile(
      @"\\.\MouClassInputInjection", // Replace with your actual device path
      FileAccess.ReadWrite,
      FileShare.Read | FileShare.Write,
      IntPtr.Zero,
      FileMode.Open,
      0,
      IntPtr.Zero
    );

    return handle;
  }

  [DllImport("kernel32.dll", SetLastError = true)]
  private static extern SafeFileHandle CreateFile(
    string lpFileName,
    FileAccess dwDesiredAccess,
    FileShare dwShareMode,
    IntPtr lpSecurityAttributes,
    FileMode dwCreationDisposition,
    uint dwFlagsAndAttributes,
    IntPtr hTemplateFile
  );

  [DllImport("kernel32.dll", SetLastError = true)]
  private static extern bool DeviceIoControl(
    SafeFileHandle hDevice,
    uint dwIoControlCode,
    IntPtr lpInBuffer,
    uint nInBufferSize,
    IntPtr lpOutBuffer,
    uint nOutBufferSize,
    out uint lpBytesReturned,
    IntPtr lpOverlapped
  );

  public static void InjectMouseButton(ushort buttonFlags) {
    // Implement similarly as before, ensuring proper error handling
  }

  public static void InjectMouseMovement(short deltaX, short deltaY) {
    // Implement similarly as before, ensuring proper error handling
  }
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
