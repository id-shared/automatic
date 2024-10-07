class DedicatedWorker {
  private void WorkerLoop() {
    while (IsWork) {
      (TaskQueue.TryDequeue(out Action action) ? action.Invoke : (Action)(() => Spinner.SpinOnce()))();
    }
  }

  public void Enqueue(Action action) {
    TaskQueue.Enqueue(action);
  }

  public void Stop() {
    IsWork = F;
    foreach (var worker in Workers) {
      worker.Join();
    }
  }

  public DedicatedWorker(int k) {
    TaskQueue = new LockFreeRingBuffer<Action>(1024);
    Workers = new Thread[k];
    IsWork = T;

    for (int i = 0; i < k; i++) {
      Workers[i] = new Thread(WorkerLoop) {
        IsBackground = T
      };
      Workers[i].Start();
    }
  }

  private readonly LockFreeRingBuffer<Action> TaskQueue;
  private readonly SpinWait Spinner = new SpinWait();
  private readonly Thread[] Workers;
  private volatile bool IsWork;
  private const bool F = false;
  private const bool T = true;
}

class LockFreeRingBuffer<Item> {
  public bool TryDequeue(out Item item) {
    lock (_buffer) {
      if (_head == _tail) {
        item = default!;
        return F;
      }

      item = _buffer[_head];
      _buffer[_head] = default!;
      _head = (_head + 1) % _buffer.Length;
      return T;
    }
  }

  public void Enqueue(Item item) {
    lock (_buffer) {
      _buffer[_tail] = item;
      _tail = (_tail + 1) % _buffer.Length;
    }
  }

  public LockFreeRingBuffer(int size) {
    _buffer = new Item[size];
    _head = 0;
    _tail = 0;
  }

  private readonly Item[] _buffer;
  private static int _head;
  private static int _tail;
  private const bool F = false;
  private const bool T = true;
}
