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

  bool ab(HANDLE x1, D1Control x) {
    DWORD bytes_returned;
    return DeviceIoControl(x1, 0x88883020, &x, sizeof x, nullptr, 0, &bytes_returned, nullptr);
  }

  bool yx(HANDLE x1, int y, int x) {
    return ab(x1, D1Control{
      .type = D1Control::Type::Mouse,
      .mi = MOUSE_INPUT_DATA {
        .LastX = x,
        .LastY = y,
      },
      });
  }

  bool ee(HANDLE x1, bool e) {
    ULONG buttons = e ? MOUSE_LEFT_BUTTON_DOWN : MOUSE_LEFT_BUTTON_UP;

    return ab(x1, D1Control{
      .type = D1Control::Type::Mouse,
      .mi = MOUSE_INPUT_DATA {
        .Buttons = buttons
      },
      });
  }
}
