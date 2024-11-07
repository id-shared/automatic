#pragma once
#include <windows.h>
#include <iostream>

namespace Event {
  HHOOK hHook = NULL;

  // Callback function for keyboard hook
  LRESULT CALLBACK KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam) {
    if (nCode == HC_ACTION) {
      KBDLLHOOKSTRUCT* pKeyboard = reinterpret_cast<KBDLLHOOKSTRUCT*>(lParam);

      // Detect forward slash (/) key press
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
    return CallNextHookEx(hHook, nCode, wParam, lParam);
  }

  void SetHook() {
    hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardProc, NULL, 0);
    if (hHook == NULL) {
      std::cerr << "Failed to install hook!" << std::endl;
    }
  }

  void Unhook() {
    UnhookWindowsHookEx(hHook);
  }

  int xyloid1() {
    SetHook();
    std::cout << "Press '/' to detect the key press and release. Press ESC to exit." << std::endl;

    // Message loop to keep the hook active
    MSG msg;
    while (GetMessage(&msg, NULL, 0, 0)) {
      if (msg.message == WM_KEYDOWN && msg.wParam == VK_ESCAPE) {
        break;  // Exit if ESC key is pressed
      }
      TranslateMessage(&msg);
      DispatchMessage(&msg);
    }

    Unhook();
    return 0;
  }
}
