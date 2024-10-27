#pragma once
#include "Xyloid.hpp"

namespace Xyloid1 {
  Xyloid::Xyloid xyloid_ = Xyloid::Xyloid{
    .type = Xyloid::Xyloid::Type::Keyboard,
    .ki = KEYBOARD_INPUT_DATA {},
  };

  bool act(HANDLE x, Xyloid::Xyloid v) {
    DWORD bytes_returned;
    return DeviceIoControl(x, 0x88883020, &v, sizeof v, nullptr, 0, &bytes_returned, nullptr);
  }

  bool ee(HANDLE x, USHORT e, bool a) {
    Xyloid::Xyloid xyloid = xyloid_;
    xyloid.ki.Reserved = 0;
    xyloid.ki.MakeCode = e;
    xyloid.ki.Flags = a ? KEY_MAKE : KEY_BREAK;
    xyloid.ki.ExtraInformation = 0;
    return act(x, xyloid);
  }

  bool ar(HANDLE x, bool a) {
    return ee(x, 0x4D, a);
  }

  bool al(HANDLE x, bool a) {
    return ee(x, 0x4B, a);
  }

  bool sf(HANDLE x, bool a) {
    return ee(x, 0x35, a);
  }
}
