class Program {
  static void Main() {
    try {
      // Initialize the driver, which will set it up to listen for mouse and keyboard input
      MouseInjection.Initialize();

      Console.WriteLine("Injecting mouse input...");

      // Inject a left mouse click.
      const ushort MOUSE_LEFT_BUTTON_DOWN = 0x0002;
      const ushort MOUSE_LEFT_BUTTON_UP = 0x0004;
      MouseInjection.InjectMouseButton(MOUSE_LEFT_BUTTON_DOWN);
      MouseInjection.InjectMouseButton(MOUSE_LEFT_BUTTON_UP);

      // Inject mouse movement (e.g., move by 100 pixels to the right).
      MouseInjection.InjectMouseMovement(100, 0);

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
