using System.Runtime.InteropServices;

class Time {
  public static bool Wait(double milliseconds) {
    //QueryPerformanceFrequency(out long frequency);
    //QueryPerformanceCounter(out long start);

    //double ticksToWait = (milliseconds / 1000.0) * frequency;
    //long current;

    //do {
    //  QueryPerformanceCounter(out current);
    //} while (current - start < ticksToWait);

    Thread.Sleep((int)milliseconds);

    return A.T;
  }

  [DllImport("kernel32.dll")]
  extern static bool QueryPerformanceCounter(out long lpPerformanceCount);

  [DllImport("kernel32.dll")]
  extern static bool QueryPerformanceFrequency(out long lpFrequency);
}
