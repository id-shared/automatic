#pragma once
#include <Windows.h>
#include <iostream>

namespace Ram {
  struct Detail {
    uint8_t n4;
    uint8_t n3;
    uint8_t n2;
    uint8_t n1;
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
}
