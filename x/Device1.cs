using System.Runtime.InteropServices;

class Device1 {
  public Device1(IntPtr e, string c) {
    context = new(@$"\\.\{c}");
    process = e;
    _ = Act(new ContextO(), code.CONTEXT, A.F) ? A.T : throw new InvalidOperationException(nameof(Device1));
  }

  public bool YX(int y, int x) {
    return Act(new MoveI() {
      ProcessId = process,
      IndicatorFlags = 0,
      MovementX = x,
      MovementY = y,
    }, code.MOVE, A.T);
  }

  public bool E(ushort e) {
    return e switch {
      2 => Device2.Input([KeyM.L], A.F),
      1 => Device2.Input([KeyM.L], A.T),
      _ => Device2.Input([KeyM.L], A.T),
    }; 
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
