using NAudio.Wave;
using WindowsInput;
using WindowsInput.Native;

class Program {
  static TaskCompletionSource<bool> exitSignal = new TaskCompletionSource<bool>();
  static bool isKeyPressed = false;
  static DateTime threshold = DateTime.MinValue;
  static int release_delay = 2000;

  static async Task Main(string[] args) {
    int maximum = -50;
    int iterate = 10;

    InputSimulator inputSimulator = new InputSimulator();

    using (var waveIn = new WaveInEvent()) {
      waveIn.WaveFormat = new WaveFormat(44100, 1);
      waveIn.BufferMilliseconds = iterate; // Adjust this for faster processing.

      waveIn.DataAvailable += (sender, e) => {
        float current = GetMaxVolume(e.Buffer, e.BytesRecorded);
        double needed = 20 * Math.Log10(current);

        if (needed > maximum) {
          threshold = DateTime.Now;

          if (!isKeyPressed) {
            inputSimulator.Keyboard.KeyDown(VirtualKeyCode.VK_V);
            isKeyPressed = true;
            Console.WriteLine("Pressed 'V' key");
          }
        } else if (isKeyPressed) {
          if ((DateTime.Now - threshold).TotalMilliseconds >= release_delay) {
            inputSimulator.Keyboard.KeyUp(VirtualKeyCode.VK_V);
            isKeyPressed = false;
            Console.WriteLine("Released 'V' key after delay");
          }
        }
      };

      waveIn.StartRecording();

      Console.CancelKeyPress += (sender, e) => {
        e.Cancel = true;
        exitSignal.SetResult(true);
      };

      await exitSignal.Task;

      waveIn.StopRecording();
    }
  }

  static float GetMaxVolume(byte[] buffer, int bytesRecorded) {
    float maxVolume = 0;
    for (int i = 0; i < bytesRecorded; i += 2) {
      short sample = BitConverter.ToInt16(buffer, i);
      float sample32 = sample / 32768f;

      if (sample32 < 0) { sample32 = -sample32; }
      if (sample32 > maxVolume) { maxVolume = sample32; }
    }
    return maxVolume;
  }
}
