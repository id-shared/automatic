using Microsoft.Win32.SafeHandles;

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
