class DedicatedWorker {
  private readonly LockFreeRingBuffer<Action> _workQueue;
  private readonly Thread _workerThread;
  private volatile bool _isRunning;

  public DedicatedWorker(int bufferSize) {
    _workQueue = new LockFreeRingBuffer<Action>(bufferSize);
    _workerThread = new Thread(WorkerLoop) { IsBackground = true };
    _isRunning = true;
    _workerThread.Start();
  }

  public void Enqueue(Action work) {
    _workQueue.Enqueue(work);
  }

  private void WorkerLoop() {
    SpinWait spinWait = new SpinWait();
    while (_isRunning) {
      if (_workQueue.TryDequeue(out var workItem)) {
        workItem();
      } else {
        // Avoid blocking the worker thread, spin for very short periods
        spinWait.SpinOnce();
      }
    }
  }

  public void Stop() {
    _isRunning = false;
    _workerThread.Join();
  }
}

public class LockFreeRingBuffer<T> {
  private readonly T[] _buffer;
  private readonly int _capacity;
  private volatile int _head;
  private volatile int _tail;

  public LockFreeRingBuffer(int capacity) {
    _capacity = capacity;
    _buffer = new T[capacity];
    _head = 0;
    _tail = 0;
  }

  public bool Enqueue(T item) {
    int head = _head;
    int nextHead = (head + 1) % _capacity;

    if (nextHead == _tail) {
      // Buffer is full
      return false;
    }

    _buffer[head] = item;
    _head = nextHead;
    return true;
  }

  public bool TryDequeue(out T item) {
    int tail = _tail;
    if (tail == _head) {
      // Buffer is empty
      item = default;
      return false;
    }

    item = _buffer[tail];
    _tail = (tail + 1) % _capacity;
    return true;
  }

  public bool IsEmpty => _head == _tail;
  public bool IsFull => (_head + 1) % _capacity == _tail;
}
