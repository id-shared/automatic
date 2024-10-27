class Xyloid2 {
  public bool YX(int y, int x) {
    Xyloid_ back = xyloid_;
    back.mi.LastY = y;
    back.mi.LastX = x;

    return xyloid.Act(back, A.T);
  }

  public bool E1(bool a) {
    return EE(a ? MOUSE_LEFT_BUTTON_DOWN : MOUSE_LEFT_BUTTON_UP);
  }

  public bool EE(uint e) {
    Xyloid_ back = xyloid_;
    back.mi.Buttons = e;

    return xyloid.Act(back, A.T);
  }

  public Xyloid2(Xyloid x) {
    xyloid = x;
  }

  private readonly Xyloid_ xyloid_ = new Xyloid_ {
    type = XyloidType.Mouse,
    mi = new MOUSE_INPUT_DATA()
  };
  private readonly Xyloid xyloid;

  public const uint MOUSE_RIGHT_BUTTON_DOWN = 0x0004;
  public const uint MOUSE_LEFT_BUTTON_DOWN = 0x0001;
  public const uint MOUSE_RIGHT_BUTTON_UP = 0x0008;
  public const uint MOUSE_LEFT_BUTTON_UP = 0x0002;
}
