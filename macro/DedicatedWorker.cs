class DedicatedWorker {
  private void WorkerLoop() {
    while (_running) {
      (_taskQueue.TryDequeue(out Action action) ? action.Invoke : (Action)(() => Thread.Yield()))();
    }
  }

  public void Enqueue(Action action) {
    _taskQueue.Enqueue(action);
  }

  public void Stop() {
    _running = false;
    foreach (var worker in _workers) {
      worker.Join();
    }
  }

  public DedicatedWorker(int n) {
    _workerCount = n;
    _taskQueue = new LockFreeRingBuffer<Action>(1024);
    _workers = new Thread[_workerCount];
    _running = true;

    for (int i = 0; i < _workerCount; i++) {
      _workers[i] = new Thread(WorkerLoop) {
        IsBackground = true
      };
      _workers[i].Start();
    }
  }

  private volatile bool _running;
  private readonly int _workerCount;
  private readonly Thread[] _workers;
  private readonly LockFreeRingBuffer<Action> _taskQueue;
  private const bool F = false;
  private const bool T = true;
}

class LockFreeRingBuffer<T> {
  public bool TryDequeue(out T item) {
    lock (_buffer) {
      if (_head == _tail) {
        item = default!;
        return false;
      }

      item = _buffer[_head];
      _buffer[_head] = default!;
      _head = (_head + 1) % _buffer.Length;
      return true;
    }
  }

  public void Enqueue(T item) {
    lock (_buffer) {
      _buffer[_tail] = item;
      _tail = (_tail + 1) % _buffer.Length;
    }
  }

  public LockFreeRingBuffer(int size) {
    _buffer = new T[size];
    _head = 0;
    _tail = 0;
  }

  private readonly T[] _buffer;
  private static int _head;
  private static int _tail;
}
