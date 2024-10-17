class Program {
  public static void Main() {
    try {
      //DD dd = new();

      //int ret = dd.Load("DD.dll");

      //if (ret != 1) { Console.WriteLine("Load Error"); return; }

      //ret = dd.btn(0);
      //if (ret != 1) { Console.WriteLine("Initialize Error"); return; }

      //Thread.Sleep(2000);

      //dd.btn(1);
      //Thread.Sleep(50);
      //dd.btn(2);

      Mouse mouse = new();

      mouse.I(1);
      mouse.I(2);

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
