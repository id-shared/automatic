class Program {
  static void Main() {
    try {
      Device3 mi = new();
      Console.WriteLine("Done");
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
