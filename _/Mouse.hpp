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

  D1Control d1control = D1Control{
    .type = D1Control::Type::Mouse,
    .mi = MOUSE_INPUT_DATA {},
  };

  bool act(HANDLE x, D1Control v) {
    DWORD bytes_returned;
    return DeviceIoControl(x, 0x88883020, &v, sizeof v, nullptr, 0, &bytes_returned, nullptr);
  }

  bool ee(HANDLE x, ULONG e) {
    D1Control control = d1control;
    control.mi.Buttons = e;
    return act(x, control);
  }

  bool e1(HANDLE x, bool a) {
    return ee(x, a ? MOUSE_LEFT_BUTTON_DOWN : MOUSE_LEFT_BUTTON_UP);
  }

  bool e2(HANDLE x, bool a) {
    return ee(x, a ? MOUSE_RIGHT_BUTTON_DOWN : MOUSE_RIGHT_BUTTON_UP);
  }

  bool yx(HANDLE x, int e1, int e) {
    D1Control control = d1control;
    control.mi.LastY = e1;
    control.mi.LastX = e;
    return act(x, control);
  }

  bool zh(HANDLE x, int e) {
    D1Control control = d1control;
    control.mi.ButtonFlags = MOUSE_HWHEEL;
    control.mi.ButtonData = e;
    return act(x, control);
  }

  bool zv(HANDLE x, int e) {
    D1Control control = d1control;
    control.mi.ButtonFlags = MOUSE_WHEEL;
    control.mi.ButtonData = e;
    return act(x, control);
  }
}
