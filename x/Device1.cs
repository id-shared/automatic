using System.Runtime.InteropServices;

class Device1 {
  public Device1(string c) {
    context = new(@$"\??\ROOT#SYSTEM#0001#{{{c}}}");
    Console.WriteLine(c);
    Console.WriteLine(Act(new MOUSEINPUT() {
      dx = 5,
      dy = 5,
    }, 0x2A2010, A.T));
  }

  public bool YX(int y, int x) {
    return A.T;
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

  private readonly Context context;
}

[StructLayout(LayoutKind.Sequential)]
public struct MOUSEINPUT {
  public int dx;
  public int dy;
  public int mouseData;
  public uint dwFlags;
  public uint time;
  public IntPtr dwExtraInfo;
}

// Constants for dwFlags field (Mouse Event Flags)
public static class MouseEventFlags {
  public const uint MOUSEEVENTF_MOVE = 0x0001;
  public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
  public const uint MOUSEEVENTF_LEFTUP = 0x0004;
  public const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
  public const uint MOUSEEVENTF_RIGHTUP = 0x0010;
  public const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
  public const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
  public const uint MOUSEEVENTF_XDOWN = 0x0080;
  public const uint MOUSEEVENTF_XUP = 0x0100;
  public const uint MOUSEEVENTF_WHEEL = 0x0800;
  public const uint MOUSEEVENTF_HWHEEL = 0x01000;
  public const uint MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000;
  public const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
  public const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
}
