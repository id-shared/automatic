using System.Diagnostics;

class Current {
  public static Process[] process(string c) {
    return Process.GetProcessesByName(c);
  }
}
