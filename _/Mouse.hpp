#pragma once
#include <cstdint>
#include <ntddkbd.h>
#include <ntddmou.h>
#include <windows.h>

namespace Mouse {
  struct D1Control {
    uint32_t unk1;
    enum class Type : uint32_t {
      Keyboard = 1,
      Mouse = 2
    } type;
    union {
      KEYBOARD_INPUT_DATA ki;
      MOUSE_INPUT_DATA mi;
    };
  private:
    void assert_size() {
      static_assert(sizeof D1Control == 32);
    }
  };

  bool act(HANDLE x1, D1Control x) {
    DWORD bytes_returned;
    return DeviceIoControl(x1, 0x88883020, &x, sizeof x, nullptr, 0, &bytes_returned, nullptr);
  }

  bool ee(HANDLE x1, ULONG e) {
    return act(x1, D1Control{
      .type = D1Control::Type::Mouse,
      .mi = MOUSE_INPUT_DATA {
        .Buttons = e
      },
      });
  }

  bool e1(HANDLE x1, bool e) {
    return ee(x1, e ? MOUSE_LEFT_BUTTON_DOWN : MOUSE_LEFT_BUTTON_UP);
  }

  bool e2(HANDLE x1, bool e) {
    return ee(x1, e ? MOUSE_RIGHT_BUTTON_DOWN : MOUSE_RIGHT_BUTTON_UP);
  }

  bool yx(HANDLE x1, int y, int x) {
    return act(x1, D1Control{
      .type = D1Control::Type::Mouse,
      .mi = MOUSE_INPUT_DATA {
        .LastX = x,
        .LastY = y,
      },
      });
  }
}
