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

  bool act(HANDLE x, D1Control v) {
    DWORD bytes_returned;
    return DeviceIoControl(x, 0x88883020, &v, sizeof v, nullptr, 0, &bytes_returned, nullptr);
  }

  bool ee(HANDLE x, ULONG e) {
    return act(x, D1Control{
      .type = D1Control::Type::Mouse,
      .mi = MOUSE_INPUT_DATA {
        .Buttons = e
      },
      });
  }

  bool e1(HANDLE x, bool a) {
    return ee(x, a ? MOUSE_LEFT_BUTTON_DOWN : MOUSE_LEFT_BUTTON_UP);
  }

  bool e2(HANDLE x, bool a) {
    return ee(x, a ? MOUSE_RIGHT_BUTTON_DOWN : MOUSE_RIGHT_BUTTON_UP);
  }

  bool yx(HANDLE x, int e1, int e) {
    return act(x, D1Control{
      .type = D1Control::Type::Mouse,
      .mi = MOUSE_INPUT_DATA {
        .LastX = e,
        .LastY = e1,
      },
      });
  }

  bool zz(HANDLE x, bool a) {
    return act(x, D1Control{
      .type = D1Control::Type::Mouse,
      .mi = MOUSE_INPUT_DATA {
      },
      });
  }
}
