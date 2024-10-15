using System.Diagnostics;
using System.Runtime.InteropServices;

class Time {
  public static TimeSpan Took(Action action) {
    Stopwatch stopwatch = Stopwatch.StartNew();
    action.Invoke();
    stopwatch.Stop();
    Console.WriteLine($"Action executed in: {stopwatch.Elapsed.TotalMilliseconds} ms");
    return stopwatch.Elapsed;
  }

  public static bool Ran(double ms) {
    QueryPerformanceFrequency(out long frequency);
    QueryPerformanceCounter(out long start);

    double ticksToWait = (ms / 1000.0) * frequency;
    long current;

    do {
      QueryPerformanceCounter(out current);
    } while (current - start < ticksToWait);

    return A.T;
  }

  [DllImport("kernel32.dll")]
  extern static bool QueryPerformanceCounter(out long lpPerformanceCount);

  [DllImport("kernel32.dll")]
  extern static bool QueryPerformanceFrequency(out long lpFrequency);
}
