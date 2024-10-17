using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

class Driver {
  public readonly DD dd;

  public Driver() {
    dd = new();
    Console.WriteLine($"{GetType().Name}: {dd.Load("DD.dll") == 1 && dd.btn(0) == 1}");
  }

  public bool X(int y, int x) {
    dd.movR(x, y);
    return A.T;
  }

  public bool I(int x) {
    dd.btn(x);
    return A.T;
  }
}

class DD {
  public pDD_btn btn { get; private set; } = null!;
  public pDD_whl whl { get; private set; } = null!;
  public pDD_mov mov { get; private set; } = null!;
  public pDD_movR movR { get; private set; } = null!;
  public pDD_key key { get; private set; } = null!;
  public pDD_str str { get; private set; } = null!;
  public pDD_todc todc { get; private set; } = null!;

  public delegate int pDD_btn(int btn);
  public delegate int pDD_whl(int whl);
  public delegate int pDD_key(int ddcode, int flag);
  public delegate int pDD_mov(int x, int y);
  public delegate int pDD_movR(int dx, int dy);
  public delegate int pDD_str(string str);
  public delegate int pDD_todc(int vkcode);

  private IntPtr _libraryHandle = IntPtr.Zero;

  ~DD() => FreeLibrary(_libraryHandle);

  public int Load(string dllFile) {
    _libraryHandle = LoadLibrary(dllFile);
    return _libraryHandle == IntPtr.Zero ? -2 : LoadFunctionAddresses();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private int LoadFunctionAddresses() {
    btn = LoadFunction<pDD_btn>("DD_btn");
    whl = LoadFunction<pDD_whl>("DD_whl");
    mov = LoadFunction<pDD_mov>("DD_mov");
    movR = LoadFunction<pDD_movR>("DD_movR");
    key = LoadFunction<pDD_key>("DD_key");
    str = LoadFunction<pDD_str>("DD_str");
    todc = LoadFunction<pDD_todc>("DD_todc");

    return 1;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  private T LoadFunction<T>(string functionName) where T : Delegate {
    IntPtr ptr = GetProcAddress(_libraryHandle, functionName);
    if (ptr == IntPtr.Zero) {
      throw new InvalidOperationException($"Failed to load {functionName}");
    }
    return Marshal.GetDelegateForFunctionPointer<T>(ptr);
  }

  [DllImport("kernel32.dll", SetLastError = true)]
  private static extern IntPtr LoadLibrary(string dllFile);

  [DllImport("kernel32.dll", SetLastError = true)]
  private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

  [DllImport("kernel32.dll", SetLastError = true)]
  private static extern bool FreeLibrary(IntPtr hModule);
}

enum KeyModifiers {
  None = 0,
  Alt = 1,
  Control = 2,
  Shift = 4,
  Windows = 8
}
