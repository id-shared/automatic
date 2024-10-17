class Program {
  public static void Main() {
    try {
      CDD DD = new();

      DD.Load(@"C:\Users\x\Downloads\dd.2024.07\dd.43390\dd43390.dll");

      // Example usage: Initialize, move mouse, and simulate a key press
      int status = DD.btn(1);  // Initialize DD.dll or mouse click

      if (status != 1) {
        Console.WriteLine("DD.dll initialization failed.");
        return;
      }

      // Move the mouse to (500, 500)
      DD.mov(500, 500);

      // Simulate pressing the 'A' key (assuming the virtual keycode for 'A' is 601)
      DD.key(601, 1);  // Key down
      DD.key(601, 2);  // Key up

      Console.WriteLine("Mouse moved and key pressed successfully.");
      //Thread.Sleep(2000);
      //Mouse.H(1);
      //Mouse.H(2);
      //Perform _ = new();
    } catch (Exception ex) {
      Console.WriteLine($"Error: {ex.Message}");
    } finally {
      Console.ReadKey();
    }
  }
}

//public static DateTime since = DateTime.MinValue;
//public static bool is_v_held = F;

//public static bool SubscribeVoice() {
//  var waveIn = new WaveInEvent {
//    WaveFormat = new WaveFormat(44100, 1),
//    BufferMilliseconds = 100
//  };

//  waveIn.DataAvailable += (sender, e) => {
//    float current = HighestVolume(e.Buffer, e.BytesRecorded);
//    double needed = 20 * Math.Log10(current);

//    if (needed > -40 && !is_v_held) {
//      since = DateTime.Now;
//      Keyboard.Emulate(Key.V, T);
//      is_v_held = true;
//    } else if (is_v_held && (DateTime.Now - since).TotalMilliseconds >= 2000) {
//      Keyboard.Emulate(Key.V, F);
//      is_v_held = false;
//    }
//  };

//  waveIn.StartRecording();
//  return T;
//}

//public static float HighestVolume(byte[] buffer, int bytesRecorded) {
//  float maxVolume = 0;
//  for (int i = 0; i < bytesRecorded; i += 2) {
//    short sample = BitConverter.ToInt16(buffer, i);
//    float sample32 = sample / 32768f;

//    if (sample32 < 0) { sample32 = -sample32; }
//    if (sample32 > maxVolume) { maxVolume = sample32; }
//  }
//  return maxVolume;
//}
