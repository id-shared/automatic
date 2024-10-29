class Xyloid1 {
  public bool EN(uint[] k, bool a) {
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

  public bool EA(ushort e, bool a) {
    Xyloid_ back = xyloid_;
    back.ki.Reserved = 0;
    back.ki.MakeCode = e;
    back.ki.Flags = a ? KEY_MAKE : KEY_BREAK;
    back.ki.ExtraInformation = 0;
    return xyloid.Act(back, A.T);
  }

  public bool Is(uint[] k) => k.All(key => (Native.GetKeyState((int)key) & 0x8000) != 0);

  public Xyloid1(Xyloid x) {
    xyloid = x;
  }

  private readonly Xyloid_ xyloid_ = new() {
    type = XyloidType.Keyboard,
    ki = new KEYBOARD_INPUT_DATA()
  };
  private readonly Xyloid xyloid;

  private readonly uint E_KEYU = 0x0002;
  private readonly uint E_KEYD = 0x0000;
  private readonly ushort KEY_BREAK = 1;
  private readonly ushort KEY_MAKE = 0;
}
