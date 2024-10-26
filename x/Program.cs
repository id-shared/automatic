class Program {
  public static void Main() {
    try {
      string a = Contact.Device(args => args.Contains("RZCONTROL"));
      Console.WriteLine(a != null ? a : "not found.");
      Console.ReadLine();
      //Perform _ = new();
    } catch (Exception ex) {
      Console.WriteLine($"Error: {ex.Message}");
    } finally {
      Console.ReadKey();
    }
  }
}
