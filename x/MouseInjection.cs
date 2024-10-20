using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

class MouseInjection {
  public MouseInjection() {
    Context context = new(@"\\.\Device1");
    Console.WriteLine("Next");
  }
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
    IntPtr lpSecurityAttributes, // Use IntPtr.Zero if not needed
    uint dwCreationDisposition,
    uint dwFlagsAndAttributes,
    IntPtr hTemplateFile // Use IntPtr.Zero if not needed
  );

  [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
  public static extern SafeFileHandle CreateFileA(
    string lpFileName,
    uint dwDesiredAccess,
    uint dwShareMode,
    IntPtr lpSecurityAttributes, // Use IntPtr.Zero if not needed
    uint dwCreationDisposition,
    uint dwFlagsAndAttributes,
    IntPtr hTemplateFile // Use IntPtr.Zero if not needed
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
