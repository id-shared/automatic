class Program {
  public static void Main() {
    try {
      const string a = "1abc05c0-c378-41b9-9cef-df1aba82b015";
      Device1 c = new(a);
      //Perform _ = new();
    } catch (Exception ex) {
      Console.WriteLine($"Error: {ex.Message}");
    } finally {
      Console.ReadKey();
    }
  }
}
