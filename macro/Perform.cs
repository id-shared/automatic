public class Perform {
  public static void Initialize(int workerCount) {
    workerThreads = new Thread[workerCount];
    for (int i = 0; i < workerCount; i++) {
      workerThreads[i] = new Thread(ProcessTasks);
      workerThreads[i].IsBackground = true;
      workerThreads[i].Start();
    }
  }

  private static void ProcessTasks() {
    while (true) {
      if (taskQueue.TryDequeue(out Action task)) {
        task();
      } else {
        Thread.Sleep(1);
      }
    }
  }

  public static void EnqueueTask(Action task) {
    if (!taskQueue.TryEnqueue(task)) {
      // Handle queue overflow
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
    int currentHead = head;
    int newHead = currentHead + 1;

    if (newHead - tail > buffer.Length) {
      return false; // Buffer is full
    }

    buffer[currentHead & mask] = item;
    Interlocked.Exchange(ref head, newHead);
    return true;
  }

  public bool TryDequeue(out T item) {
    int currentTail = tail;

    if (currentTail >= head) {
      item = default;
      return false; // Buffer is empty
    }

    item = buffer[currentTail & mask];
    Interlocked.Exchange(ref tail, currentTail + 1);
    return true;
  }

  private readonly T[] buffer;
  private readonly int mask;
  private int head;
  private int tail;
}
