using System.Runtime.InteropServices;

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
    int localHead = _head.Value;
    if (localHead == Volatile.Read(ref _tail.Value)) {
      z = default!;
      return F;
    }

    z = _buffer[localHead & _bufferMask];
    _buffer[localHead & _bufferMask] = default!;

    _head.Value = localHead + 1;
    return T;
  }

  public bool Enqueue(Action z) {
    int localTail = _tail.Value;
    int nextTail = (localTail + 1);

    if (nextTail == Volatile.Read(ref _head.Value) + _buffer.Length) {
      return F;
    }

    _buffer[localTail & _bufferMask] = z;

    _tail.Value = nextTail;
    return T;
  }

  public LockFreeRingBuffer(int size) {
    if ((size & (size - 1)) != 0) {
      throw new ArgumentException("Size must be a power of 2");
    }

    _buffer = new Action[size];
    _bufferMask = size - 1;
    _head = new PaddedInt();
    _tail = new PaddedInt();
    _head.Value = 0;
    _tail.Value = 0;
  }

  [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 128)]
  private struct PaddedInt {
    [FieldOffset(64)]
    public int Value;
  }

  private readonly Action[] _buffer;
  private readonly int _bufferMask;

  private PaddedInt _head;
  private PaddedInt _tail;

  private const bool F = false;
  private const bool T = true;
}
