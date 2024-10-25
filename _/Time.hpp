#pragma once
#include <Windows.h>

namespace Time {
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
}
