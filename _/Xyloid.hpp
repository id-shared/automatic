#pragma once
#include <cstdint>
#include <ntddkbd.h>
#include <ntddmou.h>
#include <windows.h>

namespace Xyloid {
  struct Xyloid {
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
      static_assert(sizeof Xyloid == 32);
    }
  };
}
