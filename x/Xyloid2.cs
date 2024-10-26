using System.Runtime.InteropServices;

class Xyloid2 {
  public bool YX(int y, int x) {
    Xyloid control = d1control;
    control.mi.LastY = y;
    control.mi.LastX = x;

    return Act(control, A.T);
  }

  public bool E1(bool a) {
    return A.T;
  }

  public bool Act<X>(X x, bool a) {
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

  public Xyloid2(string c) {
    context = new(c);
  }

  private readonly Xyloid d1control = new Xyloid {
    type = XyloidType.Mouse,
    mi = new MOUSE_INPUT_DATA()
  };
  private readonly uint CODE = 0x88883020;
  private readonly Context context;
}
