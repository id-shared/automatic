#pragma once
#include "Dll.hpp"
#include <windows.h>

typedef int (WINAPI* DD_movR)(int, int);
typedef int (WINAPI* DD_btn)(int);

namespace DD {
  struct Contact {
    DD_movR movR;
    DD_btn btn;
  };

  static Contact contact(const wchar_t* c) {
    HMODULE contact = Dll::dll(c);

    DD_movR movR = Dll::fn<DD_movR>(contact, "DD_movR");

    DD_btn btn = Dll::fn<DD_btn>(contact, "DD_btn");

    btn(0);

    return Contact{
      movR,
      btn,
    };
  }
}
