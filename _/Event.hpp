#pragma once
#include <windows.h>
#include <iostream>

namespace Event {
  HHOOK hHook;

  LRESULT CALLBACK KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam) {
    if (nCode == HC_ACTION) {
      KBDLLHOOKSTRUCT* pKeyboard = reinterpret_cast<KBDLLHOOKSTRUCT*>(lParam);

      if (pKeyboard->vkCode == VK_OEM_2) {
        if (wParam == WM_KEYDOWN) {
          std::cout << "Forward slash (/) key pressed." << std::endl;
        }
        else if (wParam == WM_KEYUP) {
          std::cout << "Forward slash (/) key released." << std::endl;
        }
      }
    }

    return CallNextHookEx(hHook, nCode, wParam, lParam);
  }

  HHOOK hook() {
    hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardProc, NULL, 0);
    return hHook == NULL ? throw hHook : hHook;
  }

  BOOL __stdcall Unhook(HHOOK x) {
    return UnhookWindowsHookEx(x);
  }

  int xyloid1() {
    HHOOK x = hook();
    MSG msg;
    while (GetMessage(&msg, NULL, 0, 0)) {
      if (msg.message == WM_KEYDOWN && msg.wParam == VK_ESCAPE) {
        break;
      }
      TranslateMessage(&msg);
      DispatchMessage(&msg);
    }

    Unhook(x);
    return 0;
  }
}
