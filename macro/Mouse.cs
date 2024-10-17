using System.Runtime.InteropServices;

class Mouse {
  public static bool Input(int y, int x) {
    Console.WriteLine(DD_movR(x, y));
    return A.T;
  }

  public static bool btn(int k) {
    Console.WriteLine(DD_btn(k));
    return A.T;
  }

  [DllImport("DD.dll")]
  private static extern int DD_movR(int x, int y);

  [DllImport("DD.dll")]
  private static extern int DD_btn(int x);
}

class DD {
  public pDD_btn btn;
  public pDD_whl whl;
  public pDD_mov mov;
  public pDD_movR movR;
  public pDD_key key;
  public pDD_str str;
  public pDD_todc todc;

  public delegate int pDD_btn(int btn);
  public delegate int pDD_whl(int whl);
  public delegate int pDD_key(int ddcode, int flag);
  public delegate int pDD_mov(int x, int y);
  public delegate int pDD_movR(int dx, int dy);
  public delegate int pDD_str(string str);
  public delegate int pDD_todc(int vkcode);

  private IntPtr m_hinst;

  ~DD() {
    if (!m_hinst.Equals(IntPtr.Zero)) {
      bool b = FreeLibrary(m_hinst);
    }
  }

  public int Load(string dllfile) {
    m_hinst = LoadLibrary(dllfile);
    if (m_hinst.Equals(IntPtr.Zero)) {
      return -2;
    } else {
      return GetDDfunAddress(m_hinst);
    }
  }

  private int GetDDfunAddress(IntPtr hinst) {
    IntPtr ptr;

    ptr = GetProcAddress(hinst, "DD_btn");
    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    btn = Marshal.GetDelegateForFunctionPointer<pDD_btn>(ptr);

    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    ptr = GetProcAddress(hinst, "DD_whl");
    whl = Marshal.GetDelegateForFunctionPointer<pDD_whl>(ptr);

    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    ptr = GetProcAddress(hinst, "DD_mov");
    mov = Marshal.GetDelegateForFunctionPointer<pDD_mov>(ptr);

    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    ptr = GetProcAddress(hinst, "DD_key");
    key = Marshal.GetDelegateForFunctionPointer<pDD_key>(ptr);

    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    ptr = GetProcAddress(hinst, "DD_movR");
    movR = Marshal.GetDelegateForFunctionPointer<pDD_movR>(ptr);

    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    ptr = GetProcAddress(hinst, "DD_str");
    str = Marshal.GetDelegateForFunctionPointer<pDD_str>(ptr);

    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    ptr = GetProcAddress(hinst, "DD_todc");
    todc = Marshal.GetDelegateForFunctionPointer<pDD_todc>(ptr);

    return 1;
  }

  [DllImport("Kernel32")]
  private static extern IntPtr LoadLibrary(string dllfile);

  [DllImport("Kernel32")]
  private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

  [DllImport("kernel32.dll")]
  private static extern bool FreeLibrary(IntPtr hModule);
}

enum KeyModifiers {
  None = 0,
  Alt = 1,
  Control = 2,
  Shift = 4,
  Windows = 8
}
