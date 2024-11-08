#pragma once
#include <functional>
#include <iostream>
#include <unordered_map>
#include <windows.h>

namespace Event {
  class KeyboardHook {
  public:
    KeyboardHook(std::function<bool(UINT, bool)> callback) : hHook(NULL), keyCallback(callback) {
      instance = this;
      setHook();
      run();
    }

    void setHook() {
      hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardProc, NULL, 0);
      if (hHook == NULL) {
        throw hHook;
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
    std::function<bool(UINT, bool)> keyCallback;
    static KeyboardHook* instance;

    static LRESULT CALLBACK KeyboardProc(int nCode, WPARAM wParam, LPARAM lParam) {
      if (nCode == HC_ACTION) {
        KBDLLHOOKSTRUCT* pKeyboard = reinterpret_cast<KBDLLHOOKSTRUCT*>(lParam);

        if (instance && instance->keyCallback) {
          return instance->keyCallback(pKeyboard->vkCode, wParam == WM_KEYDOWN) ? CallNextHookEx(NULL, nCode, wParam, lParam) : +1;
        }
      }

      return CallNextHookEx(NULL, nCode, wParam, lParam);
    }
  };

  KeyboardHook* KeyboardHook::instance = nullptr;
}
