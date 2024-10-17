class Specter {
  public readonly LockFreeRingBuffer<Func<int, bool>> queued;
  public readonly Thread thread;

  public Specter(int bufferSize) {
    queued = new LockFreeRingBuffer<Func<int, bool>>(bufferSize);
    thread = new Thread(WorkerLoop) {
      IsBackground = true
    };
    thread.Start();
  }

  public bool TryEnqueue(Func<int, bool> work) {
    return queued.TryEnqueue(work);
  }

  public void WorkerLoop() {
    SpinWait spinner = new();
    while (true) {
      if (queued.TryDequeue(out var workItem)) {
        try {
          workItem(1);
        } catch (Exception ex) {
          Console.WriteLine($"Exception message: {ex.Message}");
        }
      } else {
        spinner.SpinOnce();
      }
    }
  }
}

class LockFreeRingBuffer<T> {
  public readonly T[] _buffer;
  public readonly int _capacity;
  public volatile int _head;
  public volatile int _tail;

  public LockFreeRingBuffer(int capacity) {
    _capacity = capacity;
    _buffer = new T[capacity];
    _head = 0;
    _tail = 0;
  }

  public bool TryEnqueue(T item) {
    int head = _head;
    int nextHead = (head + 1) % _capacity;

    if (nextHead == _tail) {
      return false;
    }

    _buffer[head] = item;
    _head = nextHead;
    return true;
  }

  public bool TryDequeue(out T item) {
    int tail = _tail;
    if (tail == _head) {
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
