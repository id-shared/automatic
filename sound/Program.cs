using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

class Program {
  private const int WAVE_MAPPER = -1;
  private const int CALLBACK_FUNCTION = 0x00030000;
  private const int WIM_DATA = 0x3C0;

  private delegate void WaveInProc(IntPtr hwi, uint uMsg, int dwInstance, IntPtr dwParam1, IntPtr dwParam2);

  [DllImport("winmm.dll", SetLastError = true)]
  private static extern int waveInOpen(out IntPtr hWaveIn, int uDeviceID, ref WaveFormatEx lpFormat, WaveInProc dwCallback, int dwInstance, int dwFlags);

  [DllImport("winmm.dll", SetLastError = true)]
  private static extern int waveInStart(IntPtr hWaveIn);

  [DllImport("winmm.dll", SetLastError = true)]
  private static extern int waveInClose(IntPtr hWaveIn);

  [StructLayout(LayoutKind.Sequential)]
  private struct WaveFormatEx {
    public ushort wFormatTag;
    public ushort nChannels;
    public uint nSamplesPerSec;
    public uint nAvgBytesPerSec;
    public ushort nBlockAlign;
    public ushort wBitsPerSample;
    public ushort cbSize;
  }

  private static IntPtr waveInHandle;

  private static void WaveInCallback(IntPtr hwi, uint uMsg, int dwInstance, IntPtr dwParam1, IntPtr dwParam2) {
    if (uMsg == WIM_DATA) {
      Console.WriteLine("abc");
    }
  }

  static async Task Main(string[] args) {
    // Set up wave format
    WaveFormatEx waveFormat = new WaveFormatEx {
      wFormatTag = 1, // PCM
      nChannels = 1, // Mono
      nSamplesPerSec = 44100, // 44.1 kHz
      wBitsPerSample = 16, // 16 bits per sample
      nBlockAlign = 2, // (nChannels * wBitsPerSample) / 8
      nAvgBytesPerSec = 44100 * 2, // nSamplesPerSec * nBlockAlign
      cbSize = 0
    };

    // Open the waveIn device
    WaveInProc callback = new WaveInProc(WaveInCallback);
    int result = waveInOpen(out waveInHandle, WAVE_MAPPER, ref waveFormat, callback, 0, CALLBACK_FUNCTION);

    if (result != 0) {
      Console.WriteLine("Error initializing audio input.");
      return;
    }

    // Start recording
    result = waveInStart(waveInHandle);
    if (result != 0) {
      Console.WriteLine("Error starting audio input.");
      return;
    }

    Console.WriteLine("Listening for sound...");

    // Keep the program running until user presses Enter
    await Task.Run(() => Console.ReadLine());

    // Stop and close the waveIn device
    waveInClose(waveInHandle);
  }
}
