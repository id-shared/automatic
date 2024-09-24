using NAudio.Wave;
using WindowsInput;
using WindowsInput.Native;

class Program {
  static bool isKeyPressed = false; // Tracks whether the key is currently pressed

  static async Task Main(string[] args) {
    int thresholdDb = -50; // Set a more sensitive dB threshold
    int bufferMilliseconds = 100; // Buffer length in milliseconds

    // Create an instance of InputSimulator to simulate key presses
    InputSimulator inputSimulator = new InputSimulator();

    // Create a wave in event to capture microphone input
    using (var waveIn = new WaveInEvent()) {
      waveIn.WaveFormat = new WaveFormat(44100, 1); // Mono, 44.1kHz
      waveIn.BufferMilliseconds = bufferMilliseconds;

      waveIn.DataAvailable += (sender, e) => {
        // Analyze the sound levels in the buffer
        float maxVolume = GetMaxVolume(e.Buffer, e.BytesRecorded);
        double db = 20 * Math.Log10(maxVolume); // Convert to decibels

        // If the volume is higher than the threshold and key is not pressed, press "T"
        if (db > thresholdDb && !isKeyPressed) {
          inputSimulator.Keyboard.KeyDown(VirtualKeyCode.VK_V);
          isKeyPressed = true; // Mark the key as pressed
          Console.WriteLine("Pressed 'T' key");
        }
        // If the volume is lower than the threshold and the key is pressed, release "T"
        else if (db <= thresholdDb && isKeyPressed) {
          inputSimulator.Keyboard.KeyUp(VirtualKeyCode.VK_V);
          isKeyPressed = false; // Mark the key as not pressed
          Console.WriteLine("Released 'T' key");
        }
      };

      waveIn.StartRecording();

      Console.WriteLine("Listening for sound...");
      await Task.Run(() => Console.ReadLine());

      waveIn.StopRecording();
    }
  }

  // Function to get the max volume from audio buffer
  static float GetMaxVolume(byte[] buffer, int bytesRecorded) {
    float maxVolume = 0;
    for (int i = 0; i < bytesRecorded; i += 2) {
      // Combine two bytes into one 16-bit sample
      short sample = BitConverter.ToInt16(buffer, i);
      float sample32 = sample / 32768f; // Normalize the sample

      // Track the max sample value
      if (sample32 < 0) {
        sample32 = -sample32;
      }
      if (sample32 > maxVolume) {
        maxVolume = sample32;
      }
    }
    return maxVolume;
  }
}
