#include <functional>
#include <iostream>
#include <unordered_map>
#include <windows.h>

namespace Event {
  class KeyboardHook {
  public:
    KeyboardHook() : hHook(NULL) {
      setHook();
      run();
    }

    // Set up the keyboard hook
    void setHook() {
      hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardProc, NULL, 0);
      if (hHook == NULL) {
        std::cerr << "Failed to install hook!" << std::endl;
      }
    }

    // Unhook the keyboard hook
    void unhook() {
      if (hHook) {
        UnhookWindowsHookEx(hHook);
        hHook = NULL;
      }
    }

    // Message loop to keep the hook active
    void run() {
      MSG msg;
      std::cout << "Press '/' to detect the key press and release. Press ESC to exit." << std::endl;
      while (GetMessage(&msg, NULL, 0, 0)) {
        if (msg.message == WM_KEYDOWN && msg.wParam == VK_ESCAPE) {
          break;  // Exit if ESC key is pressed
        }
        TranslateMessage(&msg);
        DispatchMessage(&msg);
      }
    }

    ~KeyboardHook() {
      unhook();
    }

  private:
    HHOOK hHook;

    // Static callback function for keyboard hook
    static LRESULT CALLBACK KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam) {
      if (nCode == HC_ACTION) {
        KBDLLHOOKSTRUCT* pKeyboard = reinterpret_cast<KBDLLHOOKSTRUCT*>(lParam);

        // Detect forward slash (/) key press and release
        if (pKeyboard->vkCode == VK_OEM_2) {
          if (wParam == WM_KEYDOWN) {
            std::cout << "Forward slash (/) key pressed." << std::endl;
          }
          else if (wParam == WM_KEYUP) {
            std::cout << "Forward slash (/) key released." << std::endl;
          }
        }
      }

      // Call the next hook in the chain
      return CallNextHookEx(NULL, nCode, wParam, lParam);
    }
  };
}
