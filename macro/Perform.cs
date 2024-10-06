public class Perform {
  private static ManualResetEventSlim taskAvailable = new ManualResetEventSlim(false); // For signaling tasks
  private static volatile bool shutdown = false;

  public static void Initialize(int workerCount) {
    workerThreads = new Thread[workerCount];
    for (int i = 0; i < workerCount; i++) {
      workerThreads[i] = new Thread(ProcessTasks) {
        IsBackground = true // Keep background threads but manage shutdown
      };
      workerThreads[i].Start();
    }
  }

  public static void Shutdown() {
    shutdown = true;
    taskAvailable.Set(); // Ensure workers exit
    foreach (var thread in workerThreads) {
      thread.Join(); // Wait for all worker threads to finish
    }
  }

  private static void ProcessTasks() {
    while (!shutdown) {
      if (taskQueue.TryDequeue(out Action task)) {
        task();
      } else {
        taskAvailable.Wait(100); // Wait up to 100 ms or until signaled
      }
    }
  }

  public static void EnqueueTask(Action task) {
    if (taskQueue.TryEnqueue(task)) {
      taskAvailable.Set(); // Signal that a new task is available
    } else {
      // Handle queue overflow (e.g., logging or retry logic)
    }
  }

  private static readonly LockFreeRingBuffer<Action> taskQueue = new(65536);
  private static Thread[] workerThreads = Array.Empty<Thread>();
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
