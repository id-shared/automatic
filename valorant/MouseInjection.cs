using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

public static class MouseInjection {
  private static readonly DriverHandleManager _handleManager = new();
  private static bool _isInitialized = false;

  private static readonly uint IOCTL_INJECT_MOUSE_BUTTON;
  private static readonly uint IOCTL_INJECT_MOUSE_MOVEMENT;
  private static readonly uint IOCTL_INIT_DRIVER;
  private static readonly uint IOCTL_START_LISTENING;

  static MouseInjection() {
    IOCTL_INJECT_MOUSE_BUTTON = CTL_CODE(FILE_DEVICE_MOUSE, 0x801, METHOD_BUFFERED, FILE_ANY_ACCESS);
    IOCTL_INJECT_MOUSE_MOVEMENT = CTL_CODE(FILE_DEVICE_MOUSE, 0x802, METHOD_BUFFERED, FILE_ANY_ACCESS);
  }

  private const uint METHOD_BUFFERED = 0;
  private const uint FILE_DEVICE_MOUSE = 0x0000000F;
  private const uint FILE_ANY_ACCESS = 0;

  private static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access) {
    return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
  }

  // Call this method to initialize the driver and start capturing input
  public static void Initialize() {
    if (!_isInitialized) {
      using (var handle = _handleManager.GetHandle()) {
        // Call the driver's initialization IOCTL
        if (!DeviceIoControl(handle, IOCTL_INIT_DRIVER, IntPtr.Zero, 0, IntPtr.Zero, 0, out _, IntPtr.Zero)) {
          throw new InvalidOperationException("Failed to initialize driver.");
        }

        // Optionally, start listening for input events
        if (!DeviceIoControl(handle, IOCTL_START_LISTENING, IntPtr.Zero, 0, IntPtr.Zero, 0, out _, IntPtr.Zero)) {
          throw new InvalidOperationException("Failed to start listening for input.");
        }
      }
      _isInitialized = true;
      Console.WriteLine("Driver initialized and listening for input.");
    }
  }

  public static void InjectMouseButton(ushort buttonFlags) {
    if (!_isInitialized) {
      throw new InvalidOperationException("Driver not initialized. Call Initialize() first.");
    }

    using (var handle = _handleManager.GetHandle()) {
      IntPtr inBuffer = Marshal.AllocHGlobal(sizeof(ushort));
      Marshal.WriteInt16(inBuffer, (short)buttonFlags);

      if (!DeviceIoControl(handle, IOCTL_INJECT_MOUSE_BUTTON, inBuffer, (uint)sizeof(ushort), IntPtr.Zero, 0, out _, IntPtr.Zero)) {
        throw new InvalidOperationException("Failed to inject mouse button.");
      }

      Marshal.FreeHGlobal(inBuffer);
    }
  }

  public static void InjectMouseMovement(short deltaX, short deltaY) {
    if (!_isInitialized) {
      throw new InvalidOperationException("Driver not initialized. Call Initialize() first.");
    }

    using (var handle = _handleManager.GetHandle()) {
      int bufferSize = sizeof(short) * 2;
      IntPtr inBuffer = Marshal.AllocHGlobal(bufferSize);
      Marshal.WriteInt16(inBuffer, deltaX);
      Marshal.WriteInt16(inBuffer + sizeof(short), deltaY);

      if (!DeviceIoControl(handle, IOCTL_INJECT_MOUSE_MOVEMENT, inBuffer, (uint)bufferSize, IntPtr.Zero, 0, out _, IntPtr.Zero)) {
        throw new InvalidOperationException("Failed to inject mouse movement.");
      }

      Marshal.FreeHGlobal(inBuffer);
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
    out uint lpBytesReturned,
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
