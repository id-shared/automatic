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
  public pDD_btn btn;         //Mouse button 
  public pDD_whl whl;         //Mouse wheel
  public pDD_mov mov;      //Mouse move abs. 
  public pDD_movR movR;  //Mouse move rel. 
  public pDD_key key;         //Keyboard 
  public pDD_str str;            //Input visible char
  public pDD_todc todc;      //VK to ddcode

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
    btn = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_btn)) as pDD_btn;

    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    ptr = GetProcAddress(hinst, "DD_whl");
    whl = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_whl)) as pDD_whl;

    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    ptr = GetProcAddress(hinst, "DD_mov");
    mov = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_mov)) as pDD_mov;

    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    ptr = GetProcAddress(hinst, "DD_key");
    key = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_key)) as pDD_key;

    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    ptr = GetProcAddress(hinst, "DD_movR");
    movR = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_movR)) as pDD_movR;

    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    ptr = GetProcAddress(hinst, "DD_str");
    str = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_str)) as pDD_str;

    if (ptr.Equals(IntPtr.Zero)) { return -1; }
    ptr = GetProcAddress(hinst, "DD_todc");
    //todc = Marshal.GetDelegateForFunctionPointer(ptr, typeof(pDD_todc)) as pDD_todc;

    return 1;
  }

  public delegate int pDD_btn(int btn);
  public delegate int pDD_whl(int whl);
  public delegate int pDD_key(int ddcode, int flag);
  public delegate int pDD_mov(int x, int y);
  public delegate int pDD_movR(int dx, int dy);
  public delegate int pDD_str(string str);
  public delegate int pDD_todc(int vkcode);

  private IntPtr m_hinst;

  [DllImport("Kernel32")]
  private static extern IntPtr LoadLibrary(string dllfile);

  [DllImport("Kernel32")]
  private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

  [DllImport("kernel32.dll")]
  public static extern bool FreeLibrary(IntPtr hModule);
}

enum KeyModifiers {
  None = 0,
  Alt = 1,
  Control = 2,
  Shift = 4,
  Windows = 8
}
