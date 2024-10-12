class Program {
  private static void Main() {
    new Perform();
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
