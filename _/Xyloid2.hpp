#pragma once
#include "Xyloid.hpp"

namespace Xyloid2 {
  Xyloid::Xyloid xyloid_ = Xyloid::Xyloid{
    .type = Xyloid::Xyloid::Type::Mouse,
    .mi = MOUSE_INPUT_DATA {},
  };

  bool act(HANDLE x, Xyloid::Xyloid v) {
    DWORD bytes_returned;
    return DeviceIoControl(x, 0x88883020, &v, sizeof v, nullptr, 0, &bytes_returned, nullptr);
  }

  bool ea(HANDLE x, ULONG e) {
    Xyloid::Xyloid xyloid = xyloid_;
    xyloid.mi.Buttons = e;
    return act(x, xyloid);
  }

  bool e1(HANDLE x, bool a) {
    return ea(x, a ? MOUSE_LEFT_BUTTON_DOWN : MOUSE_LEFT_BUTTON_UP);
  }

  bool e2(HANDLE x, bool a) {
    return ea(x, a ? MOUSE_RIGHT_BUTTON_DOWN : MOUSE_RIGHT_BUTTON_UP);
  }

  bool yx(HANDLE x, int e1, int e) {
    Xyloid::Xyloid back = xyloid_;
    back.mi.LastY = e1;
    back.mi.LastX = e;
    return act(x, back);
  }

  bool zh(HANDLE x, int e) {
    Xyloid::Xyloid back = xyloid_;
    back.mi.ButtonFlags = MOUSE_HWHEEL;
    back.mi.ButtonData = e;
    return act(x, back);
  }

  bool zv(HANDLE x, int e) {
    Xyloid::Xyloid back = xyloid_;
    back.mi.ButtonFlags = MOUSE_WHEEL;
    back.mi.ButtonData = e;
    return act(x, back);
  }
}
