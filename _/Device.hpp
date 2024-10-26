#pragma once
#include <windows.h>

namespace Device {
  HANDLE driver(LPCWSTR c) {
    HANDLE device = CreateFileW(c, GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
    return device != INVALID_HANDLE_VALUE ? device : throw device;
  }
}
