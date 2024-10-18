class Program {
  public static void Main() {
    try {
      Perform _ = new();
    } catch (Exception ex) {
      Console.WriteLine($"Error: {ex.Message}");
    } finally {
      Console.ReadKey();
    }
  }
}
