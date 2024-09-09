using System;
using System.Runtime.InteropServices;

class Test {
  // Import the MapVirtualKey function from the user32.dll
  [DllImport("user32.dll")]
  private static extern uint MapVirtualKey(uint uCode, uint uMapType);

  private const uint MAPVK_VSC_TO_VK = 0x00; // Scan code to virtual key code

  static void Main() {
    // Define a range for scan codes. This range is based on typical scan codes used.
    // Adjust the range as needed depending on your requirements.
    uint startScanCode = 0x00;
    uint endScanCode = 0xFF;

    Console.WriteLine("Scanning for all ConsoleKey values based on scan codes...");

    for (uint scanCode = startScanCode; scanCode <= endScanCode; scanCode++) {
      // Convert the scan code to a virtual key code
      uint virtualKeyCode = MapVirtualKey(scanCode, MAPVK_VSC_TO_VK);

      // Convert the virtual key code to ConsoleKey
      if (Enum.IsDefined(typeof(ConsoleKey), (int)virtualKeyCode)) {
        ConsoleKey consoleKey = (ConsoleKey)virtualKeyCode;
        Console.WriteLine($"Scan Code: {scanCode:X2}, ConsoleKey: {consoleKey}");
      }
    }
  }
}
