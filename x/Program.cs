class Program {
  public static void Main() {
    try {
      Perform _ = new(Contact.Device(args => args.Contains("RZCONTROL")));
    } catch (Exception ex) {
      Console.WriteLine($"Error: {ex.Message}");
    } finally {
      Console.ReadKey();
    }
  }
}
