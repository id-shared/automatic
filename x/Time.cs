using System.Diagnostics;

class Time {
  public static TimeSpan Took(Action action) {
    Stopwatch tracker = Stopwatch.StartNew();
    action.Invoke();
    tracker.Stop();
    Console.WriteLine($"{tracker.Elapsed.TotalMilliseconds} ms.");
    return tracker.Elapsed;
  }

  public static bool XO(double ms) {
    Native.QueryPerformanceFrequency(out long frequency);
    Native.QueryPerformanceCounter(out long start);

    double ticksToWait = (ms / 1000.0) * frequency;
    long current;

    do {
      Native.QueryPerformanceCounter(out current);
    } while (current - start < ticksToWait);

    return A.T;
  }
}
