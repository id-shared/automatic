using System;
using System.Threading;

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
  private readonly Action[] _buffer;
  private int _head;
  private int _tail;

  public LockFreeRingBuffer(int k) {
    if ((k & (k - 1)) != 0) {
      throw new ArgumentException("Buffer size must be a power of 2.");
    }

    _buffer = new Action[k];
    _head = 0;
    _tail = 0;
  }

  public bool TryDequeue(out Action z) {
    int currentHead;
    do {
      currentHead = _head;
      if (currentHead == _tail) {
        z = default!;
        return false; // Buffer is empty
      }
    } while (Interlocked.CompareExchange(ref _head, currentHead + 1, currentHead) != currentHead);

    z = _buffer[currentHead];
    _buffer[currentHead] = default!; // Optional: Clear the reference
    return true;
  }

  public bool Enqueue(Action z) {
    int currentTail;
    do {
      currentTail = _tail;
      int nextTail = (currentTail + 1) & (_buffer.Length - 1);
      if (nextTail == _head) {
        return false; // Buffer is full
      }
    } while (Interlocked.CompareExchange(ref _tail, currentTail + 1, currentTail) != currentTail);

    _buffer[currentTail] = z;
    return true;
  }
}
