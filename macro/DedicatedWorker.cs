class WorkerPool {
  readonly DedicatedWorker[] _workers;
  int _nextWorkerIndex = 0;

  public WorkerPool(int workerCount, int bufferSize) {
    _workers = new DedicatedWorker[workerCount];
    for (int i = 0; i < workerCount; i++) {
      _workers[i] = new DedicatedWorker(bufferSize);
    }
  }

  public bool TryEnqueue(Action work) {
    int index = Interlocked.Increment(ref _nextWorkerIndex) % _workers.Length;
    return _workers[index].TryEnqueue(work);
  }
}

class DedicatedWorker {
  readonly LockFreeRingBuffer<Action> _workQueue;
  readonly Thread _workerThread;
  volatile bool _isRunning;

  public DedicatedWorker(int bufferSize) {
    _workQueue = new LockFreeRingBuffer<Action>(bufferSize);
    _workerThread = new Thread(WorkerLoop) {
      IsBackground = true
    };
    _isRunning = true;
    _workerThread.Start();
  }

  public bool TryEnqueue(Action work) {
    return _workQueue.TryEnqueue(work);
  }

  public void WorkerLoop() {
    SpinWait spinner = new();
    while (_isRunning) {
      if (_workQueue.TryDequeue(out var workItem)) {
        workItem();
      } else {
        spinner.SpinOnce();
      }
    }
  }
}

class LockFreeRingBuffer<T> {
  readonly T[] _buffer;
  readonly int _capacity;
  volatile int _head;
  volatile int _tail;

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
