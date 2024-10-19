class Program {
  static void Main() {
    try {
      MouseInjection.Initialize();
      Console.WriteLine("Mouse input injected successfully.");
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
