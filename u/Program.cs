using System.Runtime.InteropServices;

class Program {
  static void Main(string[] args) {
    string a = DeviceFinder.FindDevice(args => args.Contains("ROOT#S"));
    Console.WriteLine(a);
    Console.ReadLine();
  }
}

public class DeviceFinder {
  const uint STATUS_SUCCESS = 0x00000000;
  const uint STATUS_MORE_ENTRIES = 0x00000105;
  const uint STATUS_BUFFER_TOO_SMALL = 0xC0000023;
  const uint DIRECTORY_QUERY = 0x0001;

  [DllImport("ntdll.dll")]
  private static extern int NtOpenDirectoryObject(
      out IntPtr DirectoryHandle,
      uint DesiredAccess,
      ref OBJECT_ATTRIBUTES ObjectAttributes
  );

  [DllImport("ntdll.dll")]
  private static extern int NtQueryDirectoryObject(
      IntPtr DirectoryHandle,
      IntPtr Buffer,
      uint Length,
      bool ReturnSingleEntry,
      bool RestartScan,
      ref uint Context,
      out uint ReturnLength
  );

  [StructLayout(LayoutKind.Sequential)]
  public struct UNICODE_STRING {
    public ushort Length;
    public ushort MaximumLength;
    public IntPtr Buffer;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct OBJECT_ATTRIBUTES {
    public int Length;
    public IntPtr RootDirectory;
    public IntPtr ObjectName;
    public uint Attributes;
    public IntPtr SecurityDescriptor;
    public IntPtr SecurityQualityOfService;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct OBJECT_DIRECTORY_INFORMATION {
    public UNICODE_STRING Name;
    public UNICODE_STRING TypeName;
  }

  [DllImport("ntdll.dll", CharSet = CharSet.Unicode)]
  private static extern void RtlInitUnicodeString(ref UNICODE_STRING DestinationString, string SourceString);

  private static string PtrToStringUni(IntPtr ptr, int length) {
    return Marshal.PtrToStringUni(ptr, length / 2); // Convert byte length to character count
  }

  public static string FindDevice(Func<string, bool> predicate) {
    string result = null;
    IntPtr dirHandle = IntPtr.Zero;

    var objName = new UNICODE_STRING();
    RtlInitUnicodeString(ref objName, @"\GLOBAL??");

    var objAttr = new OBJECT_ATTRIBUTES {
      Length = Marshal.SizeOf<OBJECT_ATTRIBUTES>(),
      ObjectName = Marshal.AllocHGlobal(Marshal.SizeOf<UNICODE_STRING>())
    };

    Marshal.StructureToPtr(objName, objAttr.ObjectName, false);

    if (NtOpenDirectoryObject(out dirHandle, DIRECTORY_QUERY, ref objAttr) == STATUS_SUCCESS) {
      uint context = 0;
      int bufferSize = 2048; // Adjust buffer size if needed
      IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
      uint returnLength = 0;

      int status = NtQueryDirectoryObject(dirHandle, buffer, (uint)bufferSize, false, true, ref context, out returnLength);
      while (status == STATUS_SUCCESS || status == STATUS_MORE_ENTRIES) {
        int index = 0;
        while (true) {
          var info = Marshal.PtrToStructure<OBJECT_DIRECTORY_INFORMATION>(IntPtr.Add(buffer, index));
          if (info.Name.Buffer == IntPtr.Zero) break;

          string name = PtrToStringUni(info.Name.Buffer, info.Name.Length);
          if (predicate(name)) {
            result = @"\??\" + name;
            break;
          }

          index += Marshal.SizeOf<OBJECT_DIRECTORY_INFORMATION>();
        }

        if (!string.IsNullOrEmpty(result) || status != STATUS_MORE_ENTRIES)
          break;

        status = NtQueryDirectoryObject(dirHandle, buffer, (uint)bufferSize, false, false, ref context, out returnLength);
      }

      Marshal.FreeHGlobal(buffer);
      CloseHandle(dirHandle);
    }

    return result;
  }

  [DllImport("kernel32.dll", SetLastError = true)]
  private static extern bool CloseHandle(IntPtr hObject);
}
