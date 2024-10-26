#pragma once
#include <functional>
#include <windows.h>

namespace Usb {
  LPCWSTR read(std::function<bool(LPCWSTR name)> z) {
    return L"";
  }
}
