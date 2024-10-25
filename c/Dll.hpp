#pragma once
#include <Windows.h>

namespace Dll {
  static HMODULE dll(const wchar_t* c) {
    HMODULE back = LoadLibraryW(c);
    return back != NULL ? back : throw "Failed to load library.";
  }

  template <typename T>
  static T fn(HMODULE x, LPCSTR c) {
    FARPROC back = GetProcAddress(x, c);
    return back != NULL ? reinterpret_cast<T>(back) : throw "Failed to retrieve function address.";
  }
}
