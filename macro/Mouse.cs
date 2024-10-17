using System.Runtime.InteropServices;

class Mouse {

  public static bool Input(int y, int x) {
    Console.WriteLine(DD_movR(x, y));
    return A.T;
  }

  public static bool H(int k) {
    Console.WriteLine(DD_btn(k));
    return A.T;
  }

  [DllImport("DD.dll")]
  private static extern int DD_movR(int x, int y);

  [DllImport("DD.dll")]
  private static extern int DD_btn(int x);
}

class DD {
  // Import DD_mov (e.g., a function to move the mouse)
  [DllImport("DD.dll", CharSet = CharSet.Unicode)]
  public static extern int DD_mov(int x, int y);

  // Import DD_key (for key presses)
  [DllImport("DD.dll", CharSet = CharSet.Unicode)]
  public static extern int DD_key(int keycode, int flag);

  // Import initialization function, if needed
  [DllImport("DD.dll", CharSet = CharSet.Unicode)]
  public static extern int DD_btn(int btn);
}
