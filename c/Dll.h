#pragma once
#include <Windows.h>

namespace Dll {
  static HMODULE LoadLibraryModule(const wchar_t* c);
  template <typename T>
  static T GetFunctionPointer(HMODULE x, const char* c);
}
