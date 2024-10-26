#pragma comment(lib, "ntdll.lib")
#pragma once
#include <cstdint>
#include <functional>
#include <iostream>
#include <mutex>
#include <string>
#include <vector>
#include <windows.h>
#include <winternl.h>

namespace Usb {
  LPCWSTR read(std::function<bool(std::wstring_view name)> predicate) {
    return L"";
  }
}
