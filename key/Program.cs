using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
  // Define the callback delegate
  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

  private static LowLevelKeyboardProc _proc = HookCallback;
  private static IntPtr _hookID = IntPtr.Zero;

  static void Main(string[] args)
  {
    _hookID = SetHook(_proc);

    if (_hookID == IntPtr.Zero)
    {
      Console.WriteLine("Failed to set hook!");
    }
    else
    {
      Console.WriteLine("Hook set successfully. Listening for key presses...");
    }

    // Run message loop to keep the application alive and listen to hook events
    MSG msg;
    while (GetMessage(out msg, IntPtr.Zero, 0, 0))
    {
      TranslateMessage(ref msg);
      DispatchMessage(ref msg);
    }

    UnhookWindowsHookEx(_hookID);  // This won't be hit but is included for completeness
  }

  private static IntPtr SetHook(LowLevelKeyboardProc proc)
  {
    using (Process curProcess = Process.GetCurrentProcess())
    using (ProcessModule curModule = curProcess.MainModule)
    {
      if (curModule == null)
      {
        Console.WriteLine("Error: Could not get current module.");
        return IntPtr.Zero;
      }

      IntPtr moduleHandle = GetModuleHandle(curModule.ModuleName);
      if (moduleHandle == IntPtr.Zero)
      {
        Console.WriteLine("Error getting module handle: " + Marshal.GetLastWin32Error());
        return IntPtr.Zero;
      }

      IntPtr hook = SetWindowsHookEx(WH_KEYBOARD_LL, proc, moduleHandle, 0);
      if (hook == IntPtr.Zero)
      {
        Console.WriteLine("Error setting hook: " + Marshal.GetLastWin32Error());
      }
      return hook;
    }
  }

  private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
  {
    if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
    {
      int vkCode = Marshal.ReadInt32(lParam);
      Console.WriteLine("Key pressed: " + (ConsoleKey)vkCode);
    }
    return CallNextHookEx(_hookID, nCode, wParam, lParam);
  }

  // P/Invoke declarations
  private const int WH_KEYBOARD_LL = 13;
  private const int WM_KEYDOWN = 0x0100;
  private const int WM_SYSKEYDOWN = 0x0104;

  [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

  [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool UnhookWindowsHookEx(IntPtr hhk);

  [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

  [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
  private static extern IntPtr GetModuleHandle(string lpModuleName);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool TranslateMessage(ref MSG lpMsg);

  [DllImport("user32.dll")]
  private static extern IntPtr DispatchMessage(ref MSG lpMsg);

  [StructLayout(LayoutKind.Sequential)]
  private struct MSG
  {
    public IntPtr hWnd;
    public uint message;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint time;
    public POINT pt;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct POINT
  {
    public int x;
    public int y;
  }
}
