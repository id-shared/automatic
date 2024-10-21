using System.IO;
using System;

class Program {
  static void Main(string[] args) {
    // Specify the path for the log file
    string logFilePath = "bcdedit.txt";

    try {
      // Log some information
      using (StreamWriter writer = new StreamWriter(logFilePath, true)) {
        writer.WriteLine($"{DateTime.Now}: Log entry created.");
        writer.WriteLine("This is a log message.");
      }

      Console.WriteLine("Log entry created successfully.");
    } catch (Exception ex) {
      Console.WriteLine($"An error occurred: {ex.Message}");
    }
  }
}
