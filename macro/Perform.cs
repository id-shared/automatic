public class Perform {
  private static volatile bool shutdown = false;

  public static void Initialize(int workerCount) {
    workerThreads = new Thread[workerCount];
    for (int i = 0; i < workerCount; i++) {
      workerThreads[i] = new Thread(ProcessTasks) {
        IsBackground = true // Ensure worker threads don't block the app from exiting
      };
      workerThreads[i].Start();
    }
  }

  public static void Shutdown() {
    shutdown = true;
    foreach (var thread in workerThreads) {
      thread.Join(); // Ensure all worker threads exit properly
    }
  }

  private static void ProcessTasks() {
    SpinWait spinner = new SpinWait();
    while (!shutdown) {
      if (taskQueue.TryDequeue(out Action task)) {
        task(); // Execute the task directly
        spinner.Reset(); // Reset SpinWait after task is dequeued
      } else {
        spinner.SpinOnce(); // Backoff for a very short duration if the queue is empty
      }
    }
  }

  public static void EnqueueTask(Action task) {
    if (!taskQueue.TryEnqueue(task)) {
      // Handle queue overflow (e.g., logging or retry logic)
    }
  }

  private static readonly LockFreeRingBuffer<Action> taskQueue = new(1024);
  private static Thread[] workerThreads = [];
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
