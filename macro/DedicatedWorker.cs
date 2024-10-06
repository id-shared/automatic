public class DedicatedWorker {
  private readonly int _workerCount;
  private readonly Thread[] _workers;
  private readonly LockFreeRingBuffer<Action> _taskQueue;
  private volatile bool _running;

  private void WorkerLoop() {
    while (_running) {
      if (_taskQueue.TryDequeue(out var action)) {
        try {
          action?.Invoke(); // Execute the action
        } catch (Exception ex) {
          // Handle exceptions if necessary
          Console.WriteLine($"Error executing action: {ex}");
        }
      } else {
        Thread.Yield(); // Yield CPU time if no task is available
      }
    }
  }

  public bool Enqueue(Action action) {
    if (!_running) throw new InvalidOperationException("Worker is not running.");
    _taskQueue.Enqueue(action);
    return T;
  }

  public bool Stop() {
    _running = false;
    foreach (var worker in _workers) {
      worker.Join(); // Wait for workers to finish
    }

    return T;
  }

  public DedicatedWorker(int workerCount = 16) {
    _workerCount = workerCount;
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

  private const bool F = false;
  private const bool T = true;
}

public class LockFreeRingBuffer<T> {
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
