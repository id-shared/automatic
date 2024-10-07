class DedicatedWorker {
  public bool Enqueue(Action action) {
    return TaskQueue.Enqueue(action);
  }

  private void Loop() {
    while (T) {
      (TaskQueue.TryDequeue(out Action action) ? action.Invoke : (Action)(() => Spinner.SpinOnce()))();
    }
  }

  public DedicatedWorker(int k) {
    TaskQueue = new LockFreeRingBuffer<Action>(1024);
    Workers = new Thread[k];

    for (int i = 0; i < k; i++) {
      Workers[i] = new Thread(Loop) {
        IsBackground = T
      };
      Workers[i].Start();
    }
  }

  private readonly LockFreeRingBuffer<Action> TaskQueue;
  private readonly SpinWait Spinner = new();
  private readonly Thread[] Workers;
  private const bool F = false;
  private const bool T = true;
}

class LockFreeRingBuffer<Action> {
  public bool TryDequeue(out Action z) {
    lock (_buffer) {
      if (_head == _tail) {
        z = default!;
        return F;
      }

      z = _buffer[_head];
      _buffer[_head] = default!;
      _head = (_head + 1) % _buffer.Length;
      return T;
    }
  }

  public bool Enqueue(Action z) {
    lock (_buffer) {
      _buffer[_tail] = z;
      _tail = (_tail + 1) % _buffer.Length;
    }
    return T;
  }

  public LockFreeRingBuffer(int k) {
    _buffer = new Action[k];
    _head = 0;
    _tail = 0;
  }

  private readonly Action[] _buffer;
  private static int _head;
  private static int _tail;
  private const bool F = false;
  private const bool T = true;
}
