#pragma once
#include "Xyloid.hpp"

namespace Xyloid2 {
  Xyloid::Xyloid d1control = Xyloid::Xyloid{
    .type = Xyloid::Xyloid::Type::Mouse,
    .mi = MOUSE_INPUT_DATA {},
  };

  bool act(HANDLE x, Xyloid::Xyloid v) {
    DWORD bytes_returned;
    return DeviceIoControl(x, 0x88883020, &v, sizeof v, nullptr, 0, &bytes_returned, nullptr);
  }

  bool ee(HANDLE x, ULONG e) {
    Xyloid::Xyloid control = d1control;
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
    Xyloid::Xyloid control = d1control;
    control.mi.LastY = e1;
    control.mi.LastX = e;
    return act(x, control);
  }

  bool zh(HANDLE x, int e) {
    Xyloid::Xyloid control = d1control;
    control.mi.ButtonFlags = MOUSE_HWHEEL;
    control.mi.ButtonData = e;
    return act(x, control);
  }

  bool zv(HANDLE x, int e) {
    Xyloid::Xyloid control = d1control;
    control.mi.ButtonFlags = MOUSE_WHEEL;
    control.mi.ButtonData = e;
    return act(x, control);
  }
}
