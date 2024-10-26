using System.Runtime.InteropServices;

public class Contact {
  const uint STATUS_SUCCESS = 0x00000000;
  const uint STATUS_MORE_ENTRIES = 0x00000105;
  const uint STATUS_BUFFER_TOO_SMALL = 0xC0000023;
  const uint DIRECTORY_QUERY = 0x0001;

  private static string PtrToStringUni(IntPtr ptr, int length) {
    return Marshal.PtrToStringUni(ptr, length / 2);
  }

  public static string Device(Func<string, bool> predicate) {
    string result = null;
    IntPtr dirHandle = IntPtr.Zero;

    var objName = new Native.UNICODE_STRING();
    Native.RtlInitUnicodeString(ref objName, @"\GLOBAL??");

    var objAttr = new Native.OBJECT_ATTRIBUTES {
      Length = Marshal.SizeOf<Native.OBJECT_ATTRIBUTES>(),
      ObjectName = Marshal.AllocHGlobal(Marshal.SizeOf<Native.UNICODE_STRING>())
    };

    Marshal.StructureToPtr(objName, objAttr.ObjectName, false);

    if (Native.NtOpenDirectoryObject(out dirHandle, DIRECTORY_QUERY, ref objAttr) == STATUS_SUCCESS) {
      uint context = 0;
      int bufferSize = 2048; // Adjust buffer size if needed
      IntPtr buffer = Marshal.AllocHGlobal(bufferSize);
      uint returnLength = 0;

      int status = Native.NtQueryDirectoryObject(dirHandle, buffer, (uint)bufferSize, false, true, ref context, out returnLength);
      while (status == STATUS_SUCCESS || status == STATUS_MORE_ENTRIES) {
        int index = 0;
        while (true) {
          var info = Marshal.PtrToStructure<Native.OBJECT_DIRECTORY_INFORMATION>(IntPtr.Add(buffer, index));
          if (info.Name.Buffer == IntPtr.Zero) break;

          string name = PtrToStringUni(info.Name.Buffer, info.Name.Length);
          if (predicate(name)) {
            result = @"\??\" + name;
            break;
          }

          index += Marshal.SizeOf<Native.OBJECT_DIRECTORY_INFORMATION>();
        }

        if (!string.IsNullOrEmpty(result) || status != STATUS_MORE_ENTRIES)
          break;

        status = Native.NtQueryDirectoryObject(dirHandle, buffer, (uint)bufferSize, false, false, ref context, out returnLength);
      }

      Marshal.FreeHGlobal(buffer);
      Native.CloseHandle(dirHandle);
    }

    return result;
  }
}
