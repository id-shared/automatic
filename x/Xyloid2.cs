using System.Runtime.InteropServices;

class Xyloid2 {
  public bool YX(int y, int x) {
    Xyloid control = xyloid;
    control.mi.LastY = y * -1;
    control.mi.LastX = x;

    return Act(control, A.T);
  }

  public bool E1(bool a) {
    return EE(a ? MOUSE_LEFT_BUTTON_DOWN : MOUSE_LEFT_BUTTON_UP);
  }

  public bool EE(uint e) {
    Xyloid control = xyloid;
    control.mi.Buttons = e;

    return Act(control, A.T);
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

  public const uint MOUSE_RIGHT_BUTTON_UP = 0x0008;
  public const uint MOUSE_LEFT_BUTTON_UP = 0x0002;

  public const uint MOUSE_RIGHT_BUTTON_DOWN = 0x0004;
  public const uint MOUSE_LEFT_BUTTON_DOWN = 0x0001;

  private readonly Xyloid xyloid = new Xyloid {
    type = XyloidType.Mouse,
    mi = new MOUSE_INPUT_DATA()
  };
  private readonly uint CODE = 0x88883020;
  private readonly Context context;
}
