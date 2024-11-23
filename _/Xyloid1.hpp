#pragma once
#include "Xyloid.hpp"
#include "Time.hpp"

namespace Xyloid1 {
  Xyloid::Xyloid xyloid_ = Xyloid::Xyloid{
    .type = Xyloid::Xyloid::Type::Keyboard,
    .ki = KEYBOARD_INPUT_DATA {},
  };

  bool act(HANDLE x, Xyloid::Xyloid v) {
    DWORD bytes_returned;
    return DeviceIoControl(x, 0x88883020, &v, sizeof v, nullptr, 0, &bytes_returned, nullptr);
  }

  bool ea(HANDLE x, USHORT e, bool a) {
    Xyloid::Xyloid back = xyloid_;
    back.ki.Reserved = 0;
    back.ki.MakeCode = e;
    back.ki.Flags = a ? KEY_MAKE : KEY_BREAK;
    back.ki.ExtraInformation = 0;
    return act(x, back);
  }

  bool hold(HANDLE x, USHORT e_1, int e) {
    ea(x, e_1, true);
    Time::XO(e);
    ea(x, e_1, false);
    return true;
  }
}
