class Program {
  static void Main() {
    try {
      Device1 mi = new(Current.process("explorer").Last().Id, "Device1");
      mi.YX(-99, 99);
      mi.E(4);
      Thread.Sleep(1000);
      mi.E(8);
      Console.WriteLine("OK.");
    } catch (Exception ex) {
      Console.WriteLine($"Error: {ex.Message}");
    } finally {
      Console.ReadKey();
    }
  }
}

//class Program {
//  public static void Main() {
//    try {
//      Perform _ = new();
//    } catch (Exception ex) {
//      Console.WriteLine($"Error: {ex.Message}");
//    } finally {
//      Console.ReadKey();
//    }
//  }
//}
