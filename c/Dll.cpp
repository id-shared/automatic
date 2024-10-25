#include "Dll.h"
#include <Windows.h>

namespace DLLLoader {
  static HMODULE LoadLibraryModule(const wchar_t* c) {
    HMODULE back = LoadLibraryW(c);
    return back ? back : throw "Failed to load library";
  }

  template <typename T>
  static T GetFunctionPointer(HMODULE x, const char* c) {
    FARPROC back = GetProcAddress(x, c);
    return back ? reinterpret_cast<T>(back) : throw "Failed to retrieve function address: " << c;
  }
}
