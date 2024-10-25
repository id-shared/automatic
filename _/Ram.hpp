#pragma once

#include <iostream>
#include <Windows.h>

namespace Ram {
  using Byte = uint32_t;

  struct Detail {
    Byte n4;
    Byte n3;
    Byte n2;
    Byte n1;
  };

  static bool XO(double ms) {
    LARGE_INTEGER frequency;
    LARGE_INTEGER start;

    QueryPerformanceFrequency(&frequency);
    QueryPerformanceCounter(&start);

    double ticksToWait = (ms / 1000.0) * frequency.QuadPart;
    LARGE_INTEGER current;

    do {
      QueryPerformanceCounter(&current);
    } while (current.QuadPart - start.QuadPart < ticksToWait);

    return true;
  }

  static bool end(HANDLE shm_handle, Detail* ptr) {
    UnmapViewOfFile(ptr);
    CloseHandle(shm_handle);
    return true;
  }
}
