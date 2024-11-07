#pragma once
#include <functional>
#include <iostream>
#include <unordered_map>
#include <windows.h>

namespace Event {
  class KeyboardHook {
  public:
    KeyboardHook(std::function<void(int)> callback) : hHook(NULL), keyCallback(callback) {
      instance = this;
      setHook();
      run();
    }

    void setHook() {
      hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardProc, NULL, 0);
      if (hHook == NULL) {
        std::cerr << "Failed to install hook!" << std::endl;
      }
    }

    void unhook() {
      if (hHook) {
        UnhookWindowsHookEx(hHook);
        hHook = NULL;
      }
    }

    void run() {
      MSG msg;
      while (GetMessage(&msg, NULL, 0, 0)) {
        if (msg.message == WM_KEYDOWN && msg.wParam == VK_ESCAPE) {
          break;
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
    std::function<void(int)> keyCallback;
    static KeyboardHook* instance;

    static LRESULT CALLBACK KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam) {
      if (nCode == HC_ACTION) {
        KBDLLHOOKSTRUCT* pKeyboard = reinterpret_cast<KBDLLHOOKSTRUCT*>(lParam);

        if (pKeyboard->vkCode == VK_OEM_2) {
          if (wParam == WM_KEYDOWN) {
            std::cout << "Forward slash (/) key pressed." << std::endl;
          }
          else if (wParam == WM_KEYUP) {
            std::cout << "Forward slash (/) key released." << std::endl;
          }

          if (instance && instance->keyCallback) {
            instance->keyCallback(pKeyboard->vkCode);
          }
        }
      }

      return CallNextHookEx(NULL, nCode, wParam, lParam);
    }
  };

  KeyboardHook* KeyboardHook::instance = nullptr;
}
