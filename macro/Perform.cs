class Perform {
  private static readonly Thread[] workerThreads = new Thread[4];
  private static readonly CancellationTokenSource cancellationTokenSource = new();

  private static void ProcessTasks(CancellationToken cancellationToken) {
    while (!cancellationToken.IsCancellationRequested) {
      if (taskQueue.TryDequeue(out Action task)) {
        try {
          task();
        } catch (Exception ex) {
          Console.WriteLine($"Task execution failed: {ex.Message} {ex.Source} {ex.StackTrace}");
        }
      } else {
        Thread.Sleep(1);
      }
    }
  }

  public static void EnqueueTask(Action task) {
    if (!taskQueue.TryEnqueue(task)) {
      Console.WriteLine("Task queue is full, task was not enqueued.");
    }
  }

  public static void Shutdown() {
    cancellationTokenSource.Cancel();
    foreach (var thread in workerThreads) {
      thread.Join();
    }
  }

  public static void A(int workerCount) {
    for (int i = 0; i < workerCount; i++) {
      workerThreads[i] = new Thread(() => ProcessTasks(cancellationTokenSource.Token)) {
        IsBackground = true
      };
      workerThreads[i].Start();
    }
  }

  private static readonly LockFreeRingBuffer<Action> taskQueue = new(1024);
}

public class LockFreeRingBuffer<T> {
  public LockFreeRingBuffer(int capacity) {
    if (capacity <= 0 || (capacity & (capacity - 1)) != 0) {
      throw new ArgumentException("Capacity must be a positive power of 2.");
    }
    buffer = new T[capacity];
    mask = capacity - 1;
  }

  public bool TryEnqueue(T item) {
    int currentHead, newHead;
    do {
      currentHead = head;
      newHead = currentHead + 1;
      if (newHead - tail > buffer.Length) {
        return false; // Buffer is full
      }
    } while (Interlocked.CompareExchange(ref head, newHead, currentHead) != currentHead);

    buffer[currentHead & mask] = item;
    return true;
  }

  public bool TryDequeue(out T item) {
    int currentTail, newTail;
    do {
      currentTail = tail;
      if (currentTail >= head) {
        item = default;
        return false; // Buffer is empty
      }
      newTail = currentTail + 1;
    } while (Interlocked.CompareExchange(ref tail, newTail, currentTail) != currentTail);

    item = buffer[currentTail & mask];
    return true;
  }

  private readonly T[] buffer;
  private readonly int mask;
  private int head;
  private int tail;
}
