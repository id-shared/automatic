using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

class Device1 {
  public readonly DD dd;

  public Device1(string c) {
    dd = new();
    Console.WriteLine($"{GetType().Name}: {dd.Load(c) == 1 && dd.btn(0) == 1}");
  }

  public bool YX(int y, int x, bool a) {
    dd.movR(x, y);
    return A.T;
  }

  public bool L(bool a) {
    dd.btn(a ? 1 : 2);
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

  private IntPtr contact = IntPtr.Zero;

  ~DD() => Native.FreeLibrary(contact);

  public int Load(string dllFile) {
    contact = Native.LoadLibrary(dllFile);
    return contact == IntPtr.Zero ? -2 : LoadFunctionAddresses();
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
    IntPtr ptr = Native.GetProcAddress(contact, functionName);
    if (ptr == IntPtr.Zero) {
      throw new InvalidOperationException($"Failed to load {functionName}");
    }
    return Marshal.GetDelegateForFunctionPointer<T>(ptr);
  }
}

enum KeyModifiers {
  None = 0,
  Alt = 1,
  Control = 2,
  Shift = 4,
  Windows = 8
}
