class Xyloid1 {
  public bool EE(ushort e, bool a) {
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

  private readonly Xyloid_ xyloid_ = new Xyloid_ {
    type = XyloidType.Keyboard,
    ki = new KEYBOARD_INPUT_DATA()
  };
  private readonly Xyloid xyloid;

  private readonly ushort KEY_BREAK = 1;
  private readonly ushort KEY_MAKE = 0;
}
