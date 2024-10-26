class Xyloid1 {
  public bool EE(uint[] k, bool a) {
    Native.INPUT[] inputs = new Native.INPUT[k.Length];
    for (int i = 0; i < k.Length; i++) {
      inputs[i].type = 1;
      inputs[i].mkhi.ki.wVk = (ushort)k[i];
      inputs[i].mkhi.ki.wScan = 0;
      inputs[i].mkhi.ki.dwFlags = a ? E_KEYD : E_KEYU;
      inputs[i].mkhi.ki.time = 0;
      inputs[i].mkhi.ki.dwExtraInfo = IntPtr.Zero;
    }
    return Native.SendInput((uint)inputs.Length, inputs, Native.INPUT_SIZE) != 0;
  }

  public Xyloid1(string c) {
    //context = new(c);
  }

  public bool Is(uint[] k) => k.All(key => (Native.GetKeyState((int)key) & 0x8000) != 0);

  private readonly uint E_KEYU = 0x0002;
  private readonly uint E_KEYD = 0x0000;

  private readonly uint CODE = 0x2A2010;
  //private readonly Context context;
}
